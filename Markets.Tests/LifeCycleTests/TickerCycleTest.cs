namespace Markets.Tests.LifeCycleTests
{
    using Configuration;
    using DataModels;
    using Markets.Interfaces;
    using Markets.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class TickerCycleTest
    {
        [TestMethod()]
        public void BinanceTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BINANCE;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockBinanceOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void BitgetTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BITGET;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockBitgetOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void BitZTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BITZ;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockBITZOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void BybitTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.BYBIT;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockBybitOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void FTXTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.FTX;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockFTXOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void GATEIOTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.GATEIO;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockGateIOOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void HuobiTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.HUOBI;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockHuobiOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void MXCTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.MXC;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockMXCOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void OKExTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.OKEX;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockOKExOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }

        [TestMethod()]
        public void ZBGTickerCycleTest()
        {
            COIN_MARKET myMarket = COIN_MARKET.ZBG;
            IMarket market = MarketFactory.CreateMarket(myMarket, new Settings());

            market.ClearCommunicator();
            market.RegisterCommuniator(new MockZBGOrderBookApiCall());

            market.GetTickers().WaitOne();

            Assert.IsTrue(market.IsAvailableTradeCoin("ETH"));
            Assert.IsTrue(market.IsAvailableTradeCoin("BTC"));
        }
    }
}