namespace MarginServer
{
    using Common;
    using Configuration;
    using Database;
    using DataModels;
    using LogTrace.Interfaces;
    using Markets;
    using Markets.Interfaces;
    using OrderBookHandler;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Traders;

    internal class MainLogic
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("MainLogic");

        private static DBController mDBController;

        private static bool mPrevTradingData;
        private static long mLastTickerTime;
        private static long mLastLeverageTime;
        private static long mLastBalanceTime;
        private static int mCheckBalanceCount;
        private static long mStartTime;

        private static bool[] BALANCE_SUCCESS = new bool[Constants.MARKET_COUNT];
        private static bool[] MARKET_ENABLED = new bool[Constants.MARKET_COUNT];
        private static bool[] MARKET_AUTO_CONTROL = new bool[Constants.MARKET_COUNT];
        private static int[] MARKET_LEVERAGE = new int[Constants.MARKET_COUNT];
        private static int[] FAIL_COUNT = new int[Constants.MARKET_COUNT];
        private static int[] SUCCESS_COUNT = new int[Constants.MARKET_COUNT];

        private static MainLogic sMainLogic = null;

        private static Trader trader;

        private IList<IMarket> markets;

        private static OrderBookHandler orderBookCtrl = new OrderBookHandler();

        public static MainLogic getInstance()
        {
            if (sMainLogic == null)
            {
                sMainLogic = new MainLogic();
            }
            return sMainLogic;
        }

        private MainLogic()
        {
            initialize();
        }

        public void initialize()
        {
            mPrevTradingData = false;
            mLastTickerTime = 0;
            mLastBalanceTime = 0;
            mCheckBalanceCount = 0;
            mStartTime = TimeManager.UtcTimeMS();
            mDBController = new DBController();

            markets = new List<IMarket>()
            {
                MarketFactory.CreateMarket(COIN_MARKET.BINANCE, new  Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.HUOBI,new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BYBIT, new Settings()),
                //MarketFactory.CreateMarket(COIN_MARKET.BITGET, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.FTX, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.OKEX, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BITZ, new Settings()),
                //MarketFactory.CreateMarket(COIN_MARKET.MXC, new Settings()),
                //MarketFactory.CreateMarket(COIN_MARKET.ZBG, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.GATEIO, new Settings()),
            };

            trader = new Trader();
            orderBookCtrl = new OrderBookHandler();

            foreach (IMarket market in markets)
            {
                market.Initialize();
                mDBController.RegisterMarketDatabase(market);
                market.Settings = Settings.CreateMarketSettings(mDBController.LoadCoreOptionsByMarket(market.GetMyMarketName().ToString()));

                orderBookCtrl.RegisterMarket(market);
                trader.RegisterMarket(market);
            }

            foreach (var coinMarket in Enum.GetValues(typeof(COIN_MARKET)))
            {
                BALANCE_SUCCESS[(int)coinMarket] = false;
                MARKET_ENABLED[(int)coinMarket] = true;
                MARKET_AUTO_CONTROL[(int)coinMarket] = false;
                MARKET_LEVERAGE[(int)coinMarket] = 0;
            }
        }

        public void startLogic()
        {
            checkLogic();
        }

        public void checkFeeUpdateCondition()
        {
            bool isTradingNow = getTradingTimeDiff() < 25000 ? true : false;
            if (isTradingNow != mPrevTradingData)
            {
                mPrevTradingData = isTradingNow;
                if (!isTradingNow)
                {
                    myLogger.Info("call updateFeeData");
                }
            }
        }

        public void checkLogic()
        {
            bool checkEnabled = true;
            //updateFeeData();
            IDictionary<COIN_MARKET, Balance> curBalances = mDBController.LoadBalance();

            foreach (COIN_MARKET market in curBalances.Keys)
            {
                trader.SetBalance(market, curBalances[market]);
            }

            setLeverages();

            // 아래 로직을 무한루프돌면서 실행
            while (checkEnabled)
            {
                // 코어옵션에서 데이터를 정상적으로 읽어올때만 동작
                if (getCoreOptionData())
                {
                    // 시간서버 동기화
                    new Thread(() => TimeManager.syncNetworkTime()).Start();

                    // 밸런스를 불러옴, 불러오다가 응답없이 2초가 경과되면 다시 요청
                    if (TimeManager.UtcTimeMS() - mLastBalanceTime > 2 * 1000)
                    {
                        myLogger.Error("getBalance Time over, retry");
                        mCheckBalanceCount = 0;
                        getBalanceAndPosition();
                    }
                    else if (mCheckBalanceCount == 0)
                    {
                        getBalanceAndPosition();
                    }

                    // 1분에한번 레버리지 강제 세팅 (20으로)
                    setLeverages();

                    Thread.Sleep(1000);

                    // ticker data 주기적 업데이트
                    // getAllTickers();
                    // 하루에 한번 데일리 로그 작성
                    if (checkTimeDiff())
                    {
                        myLogger.Error("call AddPLDailyLogDataToDB!!!!!!!!!!!!!!!!!!!!!");

                        ///// Todo
                        mDBController.StorePLDailyLogDataToDB(trader.GetBalances(), trader.GetAllPosition());
                    }
                }
                else
                {
                    myLogger.Error("failed to get core option data!!!");
                    Thread.Sleep(990);
                }
            }
        }

        public void setLeverages()
        {
            long currentTime = TimeManager.UtcTimeMS();
            if (currentTime - mLastLeverageTime > 300 * 1000)
            {
                foreach (IMarket market in this.markets) 
                {
                    Task.Run(() => setLeverage(market.GetMyMarketName(), 20));
                }
                mLastLeverageTime = currentTime;
            }
        }

        public void setLeverage(COIN_MARKET market, int leverage)
        {
            string levString = orderBookCtrl.SetLeverage(market, leverage);
            if (levString != null && levString.Equals(leverage.ToString()))
            {
                myLogger.Warn($"{market} lev changed success!({levString})");
            }
            else
            {
                myLogger.Warn($"{market} lev changed failed!({levString})");
            }
        }

        public bool getCoreOptionData()
        {
            try
            {
                foreach (IMarket market in this.markets)
                {
                    market.Settings = Settings.CreateMarketSettings(mDBController.LoadCoreOptionsByMarket(market.GetMyMarketName().ToString()));
                }

                return true;
            }
            catch (Exception e)
            {
                myLogger.Error(e.ToString());
                return false;
            }
        }

        private int getMarketEnabledCount()
        {
            int count = 0;
            foreach (var coinMarket in Enum.GetValues(typeof(COIN_MARKET)))
            {
                if (MARKET_ENABLED[(int)coinMarket])
                {
                    count++;
                }
            }
            return count;
        }

        public long getTradingTimeDiff()
        {
            bool result = mDBController.LoadCoinTradingData();
            if (!result)
            {
                return 0;
            }
            long timeDiff = long.MaxValue;
            foreach (var coinType in Enum.GetValues(typeof(COIN_TYPE)))
            {
                long currentTime = TimeManager.UtcTimeMS();
                if (currentTime - Constants.tradingTime[(int)coinType] < timeDiff)
                {
                    timeDiff = currentTime - Constants.tradingTime[(int)coinType];
                }
            }
            return timeDiff;
        }

        public long getHandlePendingTimeDiff()
        {
            bool result = mDBController.LoadCoinTradingData();
            if (!result)
            {
                return 0;
            }
            long timeDiff = long.MaxValue;
            foreach (var coinType in Enum.GetValues(typeof(COIN_TYPE)))
            {
                long currentTime = TimeManager.UtcTimeMS();
                if (currentTime - Constants.handlePendingTime[(int)coinType] < timeDiff)
                {
                    timeDiff = currentTime - Constants.handlePendingTime[(int)coinType];
                }
            }
            return timeDiff;
        }

        public void getBalanceAndPosition()
        {
            mLastBalanceTime = TimeManager.UtcTimeMS();

            IList<Task> tasks = new List<Task>();

            foreach (IMarket market in markets)
            {
                tasks.Add(Task.Run(() => checkSuccess(market.GetMyMarketName(), orderBookCtrl.GetBalanceAndPosition(market.GetMyMarketName(), trader))));
                mDBController.updateMarketEnabledData(market.GetMyMarketName(), true);
            }

            Task.WaitAll(tasks.ToArray());

            mDBController.StoreBalanceDataAndPosition(trader.GetBalances(), trader.GetAllPosition());
            new Thread(() => logTotalUSDT()).Start();
        }

        public void checkSuccess(COIN_MARKET coinMarket, bool result)
        {
            mDBController.updateMarketEnabledData((COIN_MARKET)coinMarket, true);

            if (isAllBalanceSuccess())
            {
                myLogger.Warn("update balance to db");
                // TOdo
                //new Thread(() => mDBController.sendBalanceDataToServer()).Start();
                // Before
                //mLegacyDBController.sendBalanceDataToServer(trader.GetBalances(), trader.GetAllPosition());
                // After
            }

            //reset values
            for (int i = 0; i < Constants.MARKET_COUNT; i++)
            {
                BALANCE_SUCCESS[i] = false;
            }
            mCheckBalanceCount = 0;
        }

        public bool isAllBalanceSuccess()
        {
            bool result = true;
            foreach (var coinMarket in Enum.GetValues(typeof(COIN_MARKET)))
            {
                result = result && (BALANCE_SUCCESS[(int)coinMarket] || !MARKET_ENABLED[(int)coinMarket]);
                if (!result)
                {
                    break;
                }
            }
            return result;
        }

        public void logTotalUSDT()
        {
            myLogger.Warn("++++++++ USDT Balance +++++++");
            myLogger.Warn("Total USDT : " + trader.TotalBalance_USDT());
            myLogger.Warn("++++++++++++++++++++++++++++");
        }

        public bool checkTimeDiff()
        {
            DateTime lastTime = mDBController.SelectPLDailyLogDataLastDateTime(); // db에서 마지막 쓴 시간 받아옴
            myLogger.Error("lastTime : " + lastTime);
            DateTime lastTimeFixed = new DateTime(lastTime.Year, lastTime.Month, lastTime.Day, 0, 0, 0); // db 에서 받아온 시간을 년/월/일 만 남기고 0시 0분 0초로 변경
            myLogger.Error("lastTimeFixed : " + lastTimeFixed);
            DateTime currentTime = TimeManager.SyncronizedTime; // 현재 시간을 불러옴
            TimeSpan timeDiff = currentTime - lastTimeFixed; // TimeSpan class를 이용하여 시간의 차이를 구함

            myLogger.Warn(timeDiff.ToString());

            long tradingDiff = getTradingTimeDiff(); // 마지막 거래 후 얼마나 시간이 지났는지 계산
            long handlePendingDiff = getHandlePendingTimeDiff(); // 마지막 대기처리 후 시간이 얼마나 지났는지 계산

            return (timeDiff.Days >= 1) && (timeDiff.Minutes >= 1)
                && (tradingDiff > 60 * 1000) // 마지막 거래 후 60초 경과
                && (handlePendingDiff > 60 * 1000); // 마지막 대기 처리 후 60초 경과
        }
    }
}