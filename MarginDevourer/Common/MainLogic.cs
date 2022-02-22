namespace MarginDevourer
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

    public class MainLogic
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("MainLogger");

        private const string TAG = "MainLogic";

        private OrderbookUpdater mDataUpdater;

        private DBController mDBCtrl;

        private long mStartTime;

        private OrderBookHandler orderBookCtrl = null;

        private Trader myTrader = null;

        private CalculationStrategy myCalc = null;

        private IList<IMarket> myMarkets = null;

        private AutoResetEvent stopUpdateConditionCheck = new AutoResetEvent(false);

        public MainLogic()
        {
        }

        ~MainLogic()
        {
            this.stopUpdateConditionCheck.Set();
        }

        public void initialize(COIN_TYPE coinType)
        {
            mStartTime = TimeManager.UtcTimeMS();
            mDBCtrl = new DBController();

            IList<IMarket> myMarkets = new List<IMarket>();
            IList<IMarket> tMarkets = new List<IMarket>()
            {
                MarketFactory.CreateMarket(COIN_MARKET.BINANCE, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.HUOBI, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BYBIT, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BITGET, new Settings()),
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
            }

            foreach (IMarket market in tMarkets)
            {
                if (market.IsAvailableTradeCoin(coinType.ToString()))
                {
                    market.Settings = Settings.CreateMarketSettings(mDBCtrl.LoadCoreOptionsByMarket(market.GetMyMarketName().ToString()));

                    orderBookCtrl.RegisterMarket(market);
                    myTrader.RegisterMarket(market);

                    myMarkets.Add(market);
                }
            }

            tMarkets.Clear();

            mDBCtrl.LoadBalanceAndPosition(myTrader);

            myCalc.RegisterTrader(myTrader);
            orderBookCtrl.Subscribe(myCalc);
            orderBookCtrl.Subscribe(mDataUpdater);
        }

        public void Do(COIN_TYPE coinType)
        {
            Task.Run(() => checkCondition());
            orderBookCtrl.RequestOrderBook(coinType.ToString());

            myCalc.Start();

            while (true)
            {
                Thread.Sleep(1);
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
    }
}