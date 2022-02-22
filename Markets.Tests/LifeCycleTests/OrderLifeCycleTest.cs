namespace Markets.Tests.LifeCycleTests
{
    using Configuration;
    using Database;
    using DataModels;
    using Markets.Interfaces;
    using Markets.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class OrderLifeCycleTest
    {
        private MockObserverOrderbook mockObserveOrderBook = new MockObserverOrderbook();

        [TestMethod()]
        public void ZBGLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.ZBG;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.1);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockZBGOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(2379.95));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(2364));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(2379.85));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(2745));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("11585489047547210"));
        }

        [TestMethod()]
        public void BybitLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BYBIT;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.1);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();

            market.ClearCommunicator();

            market.RegisterCommuniator(new MockBybitOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(9487.00000000));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(336241.00000000));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(9487.5));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(522147.00000000));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("bd1844f-f3c0-4e10-8c25-10fea03763f6"));
        }

        [TestMethod()]
        public void BinanceLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BINANCE;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.01);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();

            market.ClearCommunicator();

            market.RegisterCommuniator(new MockBinanceOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDERBOOK_SIDE.BID)[0].Equals(4110.3));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDERBOOK_SIDE.BID)[0].Equals(431.00000000));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDERBOOK_SIDE.ASK)[0].Equals(4112.30000200));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDERBOOK_SIDE.ASK)[0].Equals(12.00000000));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("22542179"));
        }

        [TestMethod()]
        public void BitgetLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BITGET;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.1);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.ClearCommunicator();

            market.RegisterCommuniator(new MockBitgetOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(8858.00000000));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(1929.9));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(7466.0));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(49.900000000000006));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("513466539039522813"));
        }

        [TestMethod()]
        public void BitZLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BITZ;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.001);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.ClearCommunicator();

            market.RegisterCommuniator(new MockBITZOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(8202.7));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(4.863));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(57.7));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(2.820));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("710370"));
        }

        [TestMethod()]
        public void FTXLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.FTX;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.1);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.ClearCommunicator();

            market.RegisterCommuniator(new MockFTXOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(4114.25));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(6.263));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(4112.25));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(49.29));
            //Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("9596912"));
        }

        [TestMethod()]
        public void GateIOLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.GATEIO;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.0001);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.ClearCommunicator();

            market.RegisterCommuniator(new MockGateIOOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(2433.5));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(168 * 0.0001));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(2433.45));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(3964 * 0.0001));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("15675394"));
        }

        public void RealOrderLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BITZ;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            DBController dbCtrl = new DBController();

            IMarket market = MarketFactory.CreateMarket(myMarket, Settings.CreateMarketSettings(dbCtrl.LoadCoreOptionsByMarket("BITZ")));
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();

            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void HuobiLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.HUOBI;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.01);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.ClearCommunicator();

            market.RegisterCommuniator(new MockHuobiOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(13084.2));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(1.68));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(13084));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(0.38));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("784017187857760256"));
        }

        [TestMethod()]
        public void MXCLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.MXC;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.0001);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.ClearCommunicator();

            market.RegisterCommuniator(new MockMXCOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(36022.5));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(100444 * 0.0001));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(36016.5));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(33877 * 0.0001));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("102057569836905984"));
        }

        [TestMethod()]
        public void OKExLifeCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.OKEX;
            COIN_TYPE myCoinType = COIN_TYPE.BTC;

            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.1);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(myMarket, settings);
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.ClearCommunicator();

            market.RegisterCommuniator(new MockOKExOrderBookApiCall());
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);

            this.LifeCycleTest(market, myCoinType, mockSubscribeOrderInfo);

            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.buy)[0].Equals(411.8));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.buy)[0].Equals(1));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthPrice(ORDER_SIDE.sell)[0].Equals(1));
            Assert.IsTrue(mockSubscribeOrderbook.GetOrderBook().DepthQuantity(ORDER_SIDE.sell)[0].Equals(0.2));
            Assert.IsTrue(mockSubscribeOrderInfo.GetOrderInfo().OrderId.Equals("312269865356374016"));
        }

        private void LifeCycleTest(IMarket market, COIN_TYPE coinType, MockObserverOrderInfo orderInfoSubscriber)
        {
            market.GetOrderBook(coinType, false).WaitOne();
            market.PlaceOrder(coinType, 1000, 1, ORDER_SIDE.buy, ORDER_DIRECTION.OPEN, ORDER_TYPE.limit).WaitOne();
            market.GetOrderInfo(coinType, orderInfoSubscriber.GetOrderInfo().OrderId).WaitOne();
            market.CancelOrder(coinType, orderInfoSubscriber.GetOrderInfo().OrderId).WaitOne();
        }
    }
}