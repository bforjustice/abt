namespace OrderBookHandler.Tests
{
    using Configuration;
    using DataModels;
    using global::OrderBookHandler.Tests.Mocks;
    using Markets;
    using Markets.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Threading;

    [TestClass()]
    public class OrderBookHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestCleanup]
        public void CleanUp()
        {
        }

        [TestMethod()]
        public void PublishOrderbookTest()
        {
            OrderBookHandler or = new OrderBookHandler();
            MockObserverOrderbooks ob = new MockObserverOrderbooks();

            or.Subscribe(ob);

            IList<IMarket> myMarkets = new List<IMarket>()
            {
                MarketFactory.CreateMarket(COIN_MARKET.ZBG, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.GATEIO, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BINANCE, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.HUOBI, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BYBIT, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BITGET, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.OKEX, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.BITZ, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.MXC, new Settings()),
                MarketFactory.CreateMarket(COIN_MARKET.FTX, new Settings()),
            };

            foreach (IMarket market in myMarkets)
            {
                or.RegisterMarket(market);
            }

            or.RequestOrderBook(COIN_TYPE.BTC.ToString());

            while (ob.CurrentOrderBooksId() < 100)
            {
                Thread.Sleep(100);
            }

            Assert.IsTrue(true);
        }
    }
}