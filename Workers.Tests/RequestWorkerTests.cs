using Configuration;
using DataModels;
using Markets;
using Markets.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Workers.Interfaces;
using Workers.Tests.Mocks;

namespace Workers.Tests
{
    [TestClass()]
    public class RequestWorkerTests
    {
        [TestMethod()]
        public void StartTest()
        {
            MockMarket mockMarket = new MockMarket();
            IWorker worker = new RESTAPIWorker(mockMarket);
            worker.Start();
            worker.Requset(REQUEST_TYPE.ORDERBOOK, "{\"symbol\":\"BTC\"}");

            Thread.Sleep(500);
            Assert.IsTrue(mockMarket.TEST_COINTYPE.Equals("BTC"));
            worker.Stop();
        }

        [TestMethod()]
        public void BinanceRESTAPIOrderbookStartTest()
        {
            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.01);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(COIN_MARKET.BINANCE, settings);
            IWorker worker = new RESTAPIWorker(market);

            MockOrderBookControl mockOrderBookControl = new MockOrderBookControl();

            market.Subscribe(mockOrderBookControl);

            worker.Start();
            worker.Requset(REQUEST_TYPE.ORDERBOOK, "{\"symbol\":\"BTC\"}");

            while (mockOrderBookControl.Result == null)
            {
                Thread.Sleep(1);
            }

            Assert.IsTrue(mockOrderBookControl.Result.State.Equals(REQUEST_STATE.NORMAL));

            worker.Stop();
        }

        [TestMethod()]
        public void HUOBIRESTAPIOrderbookStartTest()
        {
            Settings settings = new Settings();
            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.01);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(COIN_MARKET.HUOBI, settings);
            IWorker worker = new RESTAPIWorker(market);

            MockOrderBookControl mockOrderBookControl = new MockOrderBookControl();

            market.Subscribe(mockOrderBookControl);

            worker.Start();
            worker.Requset(REQUEST_TYPE.ORDERBOOK, "{\"symbol\":\"BTC\"}");

            while (mockOrderBookControl.Result == null)
            {
                Thread.Sleep(1);
            }

            Assert.IsTrue(mockOrderBookControl.Result.State.Equals(REQUEST_STATE.NORMAL));

            worker.Stop();
        }
    }
}