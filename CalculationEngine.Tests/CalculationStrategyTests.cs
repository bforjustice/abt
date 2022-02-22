namespace CalculationEngine.Tests
{
    using CalculationEngine;
    using CalculationEngine.LegacyStrategies;
    using CalculationEngine.Strategies;
    using CalculationEngine.Tests.MockFactories;
    using CalculationEngine.Tests.Mocks;
    using Communication.Interfaces;
    using Communication.Models;
    using Configuration;
    using DataModels;
    using Markets;
    using Markets.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Traders;
    using Traders.Interfaces;

    [TestClass()]
    public class CalculationStrategyTests
    {
        [TestMethod()]
        public void MockRequestTest()
        {
            IJobSubscriber jobSub = new MockJobSubscriber();
            MockApiCommunicator mockApi = MockAPICommFacotry.CreateMockAPIComm(COIN_MARKET.BINANCE.ToString());
            mockApi.Subscribe(jobSub);

            mockApi.Reqeust(new RESTHttpRequest(REQUEST_TYPE.ORDERBOOK, null, "TEst", 1)).WaitOne();
            mockApi.Reqeust(new RESTHttpRequest(REQUEST_TYPE.PLACE_ORDER, null, "TEst", 1)).WaitOne();
            mockApi.Reqeust(new RESTHttpRequest(REQUEST_TYPE.OPEN_ORDER, null, "TEst", 1)).WaitOne();
            mockApi.Reqeust(new RESTHttpRequest(REQUEST_TYPE.CANCEL_ORDER, null, "TEst", 1)).WaitOne();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SeqOrderInfoTest()
        {
            MockObserverOrderbook mockObserveOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockObserveOrderInfo_1;
            IMarket marketObj_1 = this.CreateMockMarket(COIN_MARKET.BINANCE, mockObserveOrderbook, out mockObserveOrderInfo_1, "BINANCE_SEQ_ORDER_INFO");

            marketObj_1.GetOrderInfo(COIN_TYPE.BTC, "1917641").WaitOne();
            Assert.IsTrue(mockObserveOrderInfo_1.GetOrderInfo().PendingType.Equals(PENDING_TYPE.NEW));

            marketObj_1.GetOrderInfo(COIN_TYPE.BTC, "1917641").WaitOne();
            Assert.IsTrue(mockObserveOrderInfo_1.GetOrderInfo().PendingType.Equals(PENDING_TYPE.PARTIAL));

            marketObj_1.GetOrderInfo(COIN_TYPE.BTC, "1917641").WaitOne();
            Assert.IsTrue(mockObserveOrderInfo_1.GetOrderInfo().PendingType.Equals(PENDING_TYPE.COMPLETE));
        }

        public void OrderPairCycle_CancelCycle_Test()
        {
            MockObserverOrderbook mockObserveOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockObserveBuyOrderInfo;
            IMarket marketObj_buyMarket = this.CreateMockMarketByMockFactory(COIN_MARKET.BINANCE, mockObserveOrderbook, out mockObserveBuyOrderInfo, "BINANCE_SUCCESS_CANCEL_ORDER_INFO");

            MockObserverOrderInfo mockObserveSellOrderInfo;
            IMarket marketObj_sellMarket = this.CreateMockMarketByMockFactory(COIN_MARKET.FTX, mockObserveOrderbook, out mockObserveSellOrderInfo, "FTX_SUCCESS_CANCEL_ORDER_INFO");

            marketObj_buyMarket.GetOrderBook(COIN_TYPE.BTC, false).WaitOne();
            marketObj_sellMarket.GetOrderBook(COIN_TYPE.BTC, false).WaitOne();

            IList<OrderBook> mockOrderbookList = mockObserveOrderbook.GetOrderBooks();
            Assert.IsNotNull(mockOrderbookList);

            this.SetFundmentalValue(mockOrderbookList);
            OrderBooks orderbooks = new OrderBooks(1, mockOrderbookList, mockOrderbookList);
            AlgManager.Instance.UpdateOrderBook(orderbooks);

            Trader testTrader = new Trader();

            testTrader.RegisterMarket(marketObj_buyMarket);
            testTrader.RegisterMarket(marketObj_sellMarket);
            testTrader.SetBalance(COIN_MARKET.BINANCE, this.CreateBalance());
            testTrader.SetBalance(COIN_MARKET.FTX, this.CreateBalance());
            testTrader.SetPosition(COIN_MARKET.BINANCE, COIN_TYPE.BTC, this.CreatePosition(COIN_MARKET.BINANCE, COIN_TYPE.BTC));
            testTrader.SetPosition(COIN_MARKET.FTX, COIN_TYPE.BTC, this.CreatePosition(COIN_MARKET.FTX, COIN_TYPE.BTC));

            Data result = new Data();

            CheckValidity myLegacyAlg = new CheckValidity();
            Assert.IsTrue(myLegacyAlg.checkMargin(orderbooks,
                testTrader,
                out result
                ));

            ManagedOrderCycleAlg orderCycleAlg = new ManagedOrderCycleAlg();
            orderCycleAlg.RequestOrder(new Tuple<ITrader, Data>(testTrader, result));

            while (orderCycleAlg.GetMyOrders().Count == 0)
            {
                Thread.Sleep(10);
            }

            while (orderCycleAlg.GetMyOrders().Count != 0)
            {
                Thread.Sleep(10);
            }

            Assert.IsTrue(mockObserveSellOrderInfo.GetOrderInfo().PendingType.Equals(PENDING_TYPE.CANCELED));
            Assert.IsTrue(mockObserveBuyOrderInfo.GetOrderInfo().PendingType.Equals(PENDING_TYPE.COMPLETE));
        }

        public void OrderPairCycle_OneCycle_Test()
        {
            MockObserverOrderbook mockObserveOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockObserveBuyOrderInfo;
            IMarket marketObj_buyMarket = this.CreateMockMarket(COIN_MARKET.BINANCE, mockObserveOrderbook, out mockObserveBuyOrderInfo, "BINANCE_SUCCESS_ONCE_ORDER_INFO");

            MockObserverOrderInfo mockObserveSellOrderInfo;
            IMarket marketObj_sellMarket = this.CreateMockMarket(COIN_MARKET.FTX, mockObserveOrderbook, out mockObserveSellOrderInfo, "FTX_SUCCESS_ONCE_ORDER_INFO");

            marketObj_buyMarket.GetOrderBook(COIN_TYPE.BTC, false).WaitOne();
            marketObj_sellMarket.GetOrderBook(COIN_TYPE.BTC, false).WaitOne();

            IList<OrderBook> mockOrderbookList = mockObserveOrderbook.GetOrderBooks();
            Assert.IsNotNull(mockOrderbookList);

            this.SetFundmentalValue(mockOrderbookList);
            OrderBooks orderbooks = new OrderBooks(1, mockOrderbookList, mockOrderbookList);

            Trader testTrader = new Trader();

            testTrader.RegisterMarket(marketObj_buyMarket);
            testTrader.RegisterMarket(marketObj_sellMarket);
            testTrader.SetBalance(COIN_MARKET.BINANCE, this.CreateBalance());
            testTrader.SetBalance(COIN_MARKET.FTX, this.CreateBalance());
            testTrader.SetPosition(COIN_MARKET.BINANCE, COIN_TYPE.BTC, this.CreatePosition(COIN_MARKET.BINANCE, COIN_TYPE.BTC));
            testTrader.SetPosition(COIN_MARKET.FTX, COIN_TYPE.BTC, this.CreatePosition(COIN_MARKET.FTX, COIN_TYPE.BTC));

            Data result = new Data();

            CheckValidity myLegacyAlg = new CheckValidity();
            Assert.IsTrue(myLegacyAlg.checkMargin(orderbooks,
                testTrader,
                out result
                ));

            ManagedOrderCycleAlg orderCycleAlg = new ManagedOrderCycleAlg();
            orderCycleAlg.RequestOrder(new Tuple<ITrader, Data>(testTrader, result));

            while (orderCycleAlg.GetMyOrders().Count == 0)
            {
                Thread.Sleep(10);
            }

            while (orderCycleAlg.GetMyOrders().Count != 0)
            {
                Thread.Sleep(10);
            }

            Assert.IsTrue(mockObserveSellOrderInfo.GetOrderInfo().PendingType.Equals(PENDING_TYPE.COMPLETE));
            Assert.IsTrue(mockObserveBuyOrderInfo.GetOrderInfo().PendingType.Equals(PENDING_TYPE.COMPLETE));
        }

        [TestMethod()]
        public void CalculationStrategyTest()
        {
            Position zeroPosition_1 = this.CreateZeroPosition(COIN_MARKET.BINANCE, COIN_TYPE.BTC);
            Position zeroPosition_2 = this.CreateZeroPosition(COIN_MARKET.FTX, COIN_TYPE.BTC);

            MockObserverOrderInfo mockObserveOrderInfo_1;
            MockObserverOrderInfo mockObserveOrderInfo_2;

            MockObserverOrderbook mockObserveOrderbook = new MockObserverOrderbook();

            IMarket marketObj_1 = this.CreateMockMarket(COIN_MARKET.BINANCE, mockObserveOrderbook, out mockObserveOrderInfo_1, COIN_MARKET.BINANCE.ToString());
            IMarket marketObj_2 = this.CreateMockMarket(COIN_MARKET.FTX, mockObserveOrderbook, out mockObserveOrderInfo_2, COIN_MARKET.FTX.ToString());

            marketObj_1.GetOrderBook(COIN_TYPE.BTC, false).WaitOne();
            marketObj_2.GetOrderBook(COIN_TYPE.BTC, false).WaitOne();

            IList<OrderBook> mockOrderbookList = mockObserveOrderbook.GetOrderBooks();
            Assert.IsNotNull(mockOrderbookList);

            this.SetFundmentalValue(mockOrderbookList);
            OrderBooks orderbooks = new OrderBooks(1, mockOrderbookList, mockOrderbookList);

            Data result = new Data();
            Trader testTrader = new Trader();

            testTrader.RegisterMarket(marketObj_1);
            testTrader.RegisterMarket(marketObj_2);
            testTrader.SetBalance(COIN_MARKET.BINANCE, this.CreateBalance());
            testTrader.SetBalance(COIN_MARKET.FTX, this.CreateBalance());

            CheckValidity myLegacyAlg = new CheckValidity();
            Assert.IsTrue(myLegacyAlg.checkMargin(orderbooks,
                testTrader,
                out result
                ));

            Assert.IsTrue(true);
        }

        private IMarket CreateMockMarket(COIN_MARKET marketSymbol, MockObserverOrderbook mockSubscribeOrderbook, out MockObserverOrderInfo mockSubscribeOrderInfo, string testCase)
        {
            Settings settings = new Settings();

            settings.SetDecimalLength(COIN_TYPE.BTC, 1);
            settings.SetMinTradeValue(COIN_TYPE.BTC, 0.001);
            settings.SetOrderUnit(COIN_TYPE.BTC, 0.01);

            IMarket market = MarketFactory.CreateMarket(marketSymbol, settings);
            market.ClearCommunicator();
            market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(testCase));

            mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.Subscribe(mockSubscribeOrderInfo);
            market.Subscribe(mockSubscribeOrderbook);

            return market;
        }

        private IMarket CreateMockMarketByMockFactory(COIN_MARKET marketSymbol, MockObserverOrderbook mockSubscribeOrderbook, out MockObserverOrderInfo mockSubscribeOrderInfo, string testCase)
        {
            IMarket market = MockMarketFactory.CreateMarket(marketSymbol, new Settings(), testCase);

            mockSubscribeOrderInfo = new MockObserverOrderInfo();
            market.Subscribe(mockSubscribeOrderInfo);
            market.Subscribe(mockSubscribeOrderbook);

            return market;
        }

        private Position CreateZeroPosition(COIN_MARKET marketType, COIN_TYPE coinType)
        {
            Position position = new Position(marketType, coinType);
            position.SetPrice(POSITION_SIDE.SHORT, 0);
            position.SetQuantity(POSITION_SIDE.SHORT, 0);
            position.SetPNL(POSITION_SIDE.SHORT, 0);
            position.SetLeverage(POSITION_SIDE.SHORT, 0);

            position.SetPrice(POSITION_SIDE.LONG, 0);
            position.SetQuantity(POSITION_SIDE.LONG, 0);
            position.SetPNL(POSITION_SIDE.LONG, 0);
            position.SetLeverage(POSITION_SIDE.LONG, 0);

            return position;
        }

        private Position CreateShortPosition(COIN_MARKET marketType, COIN_TYPE coinType)
        {
            Position position = new Position(marketType, coinType);
            position.SetPrice(POSITION_SIDE.SHORT, 0);
            position.SetQuantity(POSITION_SIDE.SHORT, 0);
            position.SetPNL(POSITION_SIDE.SHORT, 0);
            position.SetLeverage(POSITION_SIDE.SHORT, 0);

            return position;
        }

        private Position CreateLongPosition(COIN_MARKET marketType, COIN_TYPE coinType)
        {
            Position position = new Position(marketType, coinType);
            position.SetPrice(POSITION_SIDE.LONG, 0);
            position.SetQuantity(POSITION_SIDE.LONG, 0);
            position.SetPNL(POSITION_SIDE.LONG, 0);
            position.SetLeverage(POSITION_SIDE.LONG, 0);

            return position;
        }

        private Balance CreateBalance()
        {
            Balance balance = new Balance();
            balance.Balance_USDT = 100000;
            return balance;
        }

        private Position CreatePosition(COIN_MARKET market, COIN_TYPE type)
        {
            Position position = new Position(market, type);
            position.SetLeverage(POSITION_SIDE.LONG, 5);
            position.SetLeverage(POSITION_SIDE.SHORT, 5);

            position.SetPNL(POSITION_SIDE.LONG, 0);
            position.SetPNL(POSITION_SIDE.SHORT, 0);

            position.SetPrice(POSITION_SIDE.LONG, 0);
            position.SetPrice(POSITION_SIDE.SHORT, 0);

            position.SetQuantity(POSITION_SIDE.LONG, 0);
            position.SetQuantity(POSITION_SIDE.SHORT, 0);

            return position;
        }

        private void SetFundmentalValue(IList<OrderBook> list)
        {
            foreach (OrderBook orderBook in list)
            {
                switch (orderBook.Market)
                {
                    case COIN_MARKET.BINANCE:
                        orderBook.OrderUnit = 1;
                        orderBook.TakerFee = 0.00036;
                        orderBook.MinTradeValue = 0.1;
                        orderBook.Bound = 1;
                        orderBook.Symbol = "BTCUSDT";
                        orderBook.MarginThreshold = 2.5;
                        orderBook.PriceDiffThreshold = 1;
                        break;

                    case COIN_MARKET.FTX:
                        orderBook.OrderUnit = 1;
                        orderBook.TakerFee = 0.0003;
                        orderBook.MinTradeValue = 0.1;
                        orderBook.Bound = 1;
                        orderBook.Symbol = "BTC-PERP";
                        orderBook.MarginThreshold = 2.5;
                        orderBook.PriceDiffThreshold = 1;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}