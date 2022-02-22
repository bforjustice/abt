namespace MDWindow.Models
{
    using CalculationEngine;
    using Common;
    using Configuration;
    using Database;
    using DataModels;
    using DataUpdater;
    using LogTrace.Interfaces;
    using Markets;
    using Markets.Interfaces;
    using OrderBookHandler;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Traders;

    public class MDMainModel : IPositionSubscriber, IOrderInfoSubscriber
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("MarketLogger");

        private OrderbookUpdater mDataUpdater;

        private DBController mDBCtrl;

        private long mStartTime;

        private OrderBookHandler orderBookCtrl = null;

        private Trader myTrader = null;

        private CalculationStrategy myCalc = null;

        private IList<IMarket> myMarkets = null;

        private AutoResetEvent stopUpdateConditionCheck = new AutoResetEvent(false);

        public void Initialize(string coinType)
        {
            mStartTime = TimeManager.UtcTimeMS();
            mDBCtrl = new DBController();

            IList<IMarket> myMarkets = new List<IMarket>();
            IList<IMarket> tMarkets = new List<IMarket>()
            {
                MarketFactory.CreateMarket(COIN_MARKET.BINANCE, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.HUOBI, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BYBIT, new Settings()),
                //MarketFactory.CreateMarket(COIN_MARKET.BITGET, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.OKEX, new Settings()),
                //MarketFactory.CreateMarket(COIN_MARKET.BITZ, new Settings()),
                //MarketFactory.CreateMarket(COIN_MARKET.MXC, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.FTX, new Settings()),
                //MarketFactory.CreateMarket(COIN_MARKET.ZBG, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.GATEIO,new Settings()),
            };

            orderBookCtrl = new OrderBookHandler();
            myTrader = new Trader();
            myCalc = new CalculationStrategy();
            mDataUpdater = new OrderbookUpdater(mDBCtrl);

            foreach (IMarket market in tMarkets)
            {
                market.Initialize();
                myLogger.Info($"Market Initialize : {market.GetMyMarketName().ToString()}");
            }

            foreach (IMarket market in tMarkets)
            {
                if (market.IsAvailableTradeCoin(coinType))
                {
                    market.Settings = Settings.CreateMarketSettings(mDBCtrl.LoadCoreOptionsByMarket(market.GetMyMarketName().ToString()));

                    orderBookCtrl.RegisterMarket(market);
                    myTrader.RegisterMarket(market);

                    market.Subscribe((IOrderInfoSubscriber)this);
                    market.Subscribe((IPositionSubscriber)this);

                    myMarkets.Add(market);
                }
            }

            tMarkets.Clear();

            mDBCtrl.LoadBalanceAndPosition(myTrader);

            myCalc.RegisterTrader(myTrader);
            orderBookCtrl.Subscribe(myCalc);
            orderBookCtrl.Subscribe(mDataUpdater);
        }

        public void StartMD(string coinType)
        {
            Task.Run(() => checkCondition());
            orderBookCtrl.RequestOrderBook(coinType);
            Thread initThread = new Thread(new ThreadStart(myCalc.Start));

            initThread.Start();
        }

        public void CallLoadPositions()
        {
            foreach (IMarket market in this.myMarkets)
            {
                market.GetPosition().WaitOne();
            }
        }

        private void checkCondition()
        {
            while (this.stopUpdateConditionCheck.WaitOne(1000))
            {
                Task.Run(() => TimeManager.syncNetworkTime());
                long currentTime = TimeManager.UtcTimeMS();

                foreach (IMarket market in myMarkets)
                {
                    market.Settings = Settings.CreateMarketSettings(mDBCtrl.LoadCoreOptionsByMarket(market.GetMyMarketName().ToString()));
                }

                mDBCtrl.LoadBalanceAndPosition(myTrader);

                if (this.myTrader.GetBalances().Count == 0 && this.myTrader.GetAllPosition().Count == 0)
                {
                    myLogger.Warn("Balance and Position data are Not available");
                }
            }
        }

        public void PublishPosition(IList<Position> positions)
        {
        }

        public void PublishOrderInfo(OrderInfo orderInfo)
        {
        }
    }
}