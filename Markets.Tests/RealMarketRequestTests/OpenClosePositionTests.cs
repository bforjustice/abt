namespace Markets.Tests.RealMarketRequestTests
{
    using Communication;
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using Markets.Interfaces;
    using Markets.Tests.Helper;
    using Markets.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Traders;
    using Traders.Interfaces;

    [TestClass()]
    public class OpenClosePositionTests
    {
        //[TestMethod()]
        public void BITGETOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.BITGET, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void BITGETClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.BITGET, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void BITGETOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.BITGET, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void BITGETClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.BITGET, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void BYBITOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.BYBIT, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void BYBITClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.BYBIT, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void BYBITOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.BYBIT, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void BYBITClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.BYBIT, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void MXCOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.MXC, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void MXCClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.MXC, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void MXCOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.MXC, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void MXCClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.MXC, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void GATEIOOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.GATEIO, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void GATEIOClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.GATEIO, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void GATEIOOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.GATEIO, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void GATEIOClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.GATEIO, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void BITZOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.BITZ, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void BITZClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.BITZ, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void BITZOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.BITZ, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void BITZClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.BITZ, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void FTXOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.FTX, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void FTXClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.FTX, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void FTXOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.FTX, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void FTXClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.FTX, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void OKEXOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.OKEX, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void OKEXClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.OKEX, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void OKEXOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.OKEX, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void OKEXClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.OKEX, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void BINANCEOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.BINANCE, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void BINANCEClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.BINANCE, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void BINANCEOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.BINANCE, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void BINANCEClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.BINANCE, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void HUOBIOpenPositionBuySideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.HUOBI, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void HUOBIClosePositionBuySideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.HUOBI, COIN_TYPE.ETH, ORDER_SIDE.buy));
        }

        //[TestMethod()]
        public void HUOBIOpenPositionSellSideTest()
        {
            Assert.IsTrue(this.OpenPositionTest(COIN_MARKET.HUOBI, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void HUOBIClosePositionSellSideTest()
        {
            Assert.IsTrue(this.ClosePositionTest(COIN_MARKET.HUOBI, COIN_TYPE.ETH, ORDER_SIDE.sell));
        }

        //[TestMethod()]
        public void GATEIORealLifeCycleTest()
        {
            this.RealLifeCycleTest(COIN_MARKET.GATEIO, COIN_TYPE.ETH);
        }

        //[TestMethod()]
        public void MarketSetLeverageTest()
        {
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            OpationQuery query = new OpationQuery();

            COIN_MARKET myTestMarket = COIN_MARKET.BINANCE;
            COIN_TYPE myTestCoinType = COIN_TYPE.BTC;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(myTestMarket, query.CreateSettingValues(myTestMarket));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            market.SetLeverage(myTestCoinType, 10).WaitOne();

            Assert.IsTrue(true);
        }

        //[TestMethod()]
        public void BinanceRealLifeCycleTest()
        {
            this.RealLifeCycleTest(COIN_MARKET.BINANCE, COIN_TYPE.ETH);
        }

        //[TestMethod()]
        public void HuobiRealLifeCycleTest()
        {
            this.RealLifeCycleTest(COIN_MARKET.HUOBI, COIN_TYPE.ETH);
        }

        //[TestMethod()]
        public void FTXRealLifeCycleTest()
        {
            this.RealLifeCycleTest(COIN_MARKET.FTX, COIN_TYPE.ETH);
        }

        //[TestMethod()]
        public void OKExRealLifeCycleTest()
        {
            this.RealLifeCycleTest(COIN_MARKET.OKEX, COIN_TYPE.ETH);
        }

        //[TestMethod()]
        public void BitZRealLifeCycleTest()
        {
            this.RealLifeCycleTest(COIN_MARKET.BITZ, COIN_TYPE.ETH);
        }

        //[TestMethod()]
        public void ParallelOrderRealLifeCycleTest()
        {
            List<Task> testList = new List<Task>()
            {
                Task.Run(() => this.ParallelRequestOrder(COIN_MARKET.GATEIO, COIN_TYPE.ETH, ORDER_SIDE.buy)),
                Task.Run(() => this.ParallelRequestOrder(COIN_MARKET.HUOBI, COIN_TYPE.ETH, ORDER_SIDE.sell)),
            };

            Task.WaitAll(testList.ToArray());

            Assert.IsTrue(true);
        }

        //[TestMethod()]
        public void GetOpenOrderInfoTest()
        {
            this.GetOrderInfoLifeCycleTest(COIN_MARKET.HUOBI, COIN_TYPE.ETH, "870805614635999232");

            Assert.IsTrue(true);
        }

        //[TestMethod()]
        public void CancelOrderInfoTest()
        {
            this.CancelInfoLifeCycleTest(COIN_MARKET.HUOBI, "870805614635999232");

            Assert.IsTrue(true);
        }

        private void RealLifeCycleTest(COIN_MARKET marketName, COIN_TYPE coinType)
        {
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            OpationQuery query = new OpationQuery();

            COIN_MARKET myTestMarket = marketName;
            COIN_TYPE myTestCoinType = coinType;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(myTestMarket, query.CreateSettingValues(myTestMarket));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            // Ticker Test
            market.GetTickers().WaitOne();

            // Cancel Test
            market.GetOrderBook(myTestCoinType, false).WaitOne();
            OrderBook orderBook = mockSubscribeOrderbook.GetOrderBook();
            Assert.IsNotNull(orderBook);

            Order openOrder = this.CreateRealOrder(ORDER_SIDE.buy, ORDER_DIRECTION.OPEN, orderBook, true);
            market.PlaceOrder(myTestCoinType, openOrder.OrderPrice, openOrder.Quantity, openOrder.Side, openOrder.Direction, openOrder.Type).WaitOne();
            OrderInfo placeOrderInfo = mockSubscribeOrderInfo.GetOrderInfo();
            Assert.IsNotNull(placeOrderInfo);

            string cancelId = mockSubscribeOrderInfo.GetOrderInfo().OrderId;
            market.CancelOrder(myTestCoinType, cancelId).WaitOne();

            market.GetOrderInfo(myTestCoinType, mockSubscribeOrderInfo.GetOrderInfo().OrderId).WaitOne();
            OrderInfo openOrderInfo = mockSubscribeOrderInfo.GetOrderInfo();
            Assert.IsNotNull(openOrderInfo);

            market.GetOrderBook(myTestCoinType, false).WaitOne();
            orderBook = mockSubscribeOrderbook.GetOrderBook();
            Assert.IsNotNull(orderBook);

            openOrder = this.CreateRealOrder(ORDER_SIDE.buy, ORDER_DIRECTION.OPEN, orderBook, false);
            market.PlaceOrder(myTestCoinType, openOrder.OrderPrice, openOrder.Quantity, openOrder.Side, openOrder.Direction, openOrder.Type).WaitOne();

            market.GetOrderInfo(myTestCoinType, mockSubscribeOrderInfo.GetOrderInfo().OrderId).WaitOne();
            openOrderInfo = mockSubscribeOrderInfo.GetOrderInfo();
            Assert.IsNotNull(openOrderInfo);

            Thread.Sleep(10000);

            Order closeOrder = this.CreateRealOrder(ORDER_SIDE.buy, ORDER_DIRECTION.CLOSE, orderBook, false);
            market.PlaceOrder(myTestCoinType, closeOrder.OrderPrice, closeOrder.Quantity, closeOrder.Side, closeOrder.Direction, closeOrder.Type).WaitOne();

            market.GetOrderInfo(myTestCoinType, mockSubscribeOrderInfo.GetOrderInfo().OrderId).WaitOne();
            OrderInfo closeOrderInfo = mockSubscribeOrderInfo.GetOrderInfo();
            Assert.IsNotNull(closeOrderInfo);
        }

        private void ParallelRequestOrder(COIN_MARKET marketName, COIN_TYPE coinType, ORDER_SIDE side)
        {
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            OpationQuery query = new OpationQuery();

            COIN_MARKET myTestMarket = marketName;
            COIN_TYPE myTestCoinType = coinType;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(myTestMarket, query.CreateSettingValues(myTestMarket));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            market.GetOrderBook(myTestCoinType, false).WaitOne();
            OrderBook orderBook = mockSubscribeOrderbook.GetOrderBook();
            Assert.IsNotNull(orderBook);

            Order openOrder = this.CreateRealOrder(side, ORDER_DIRECTION.OPEN, orderBook, false);
            market.PlaceOrder(myTestCoinType, openOrder.OrderPrice, openOrder.Quantity, openOrder.Side, openOrder.Direction, openOrder.Type).WaitOne();

            market.GetOrderInfo(myTestCoinType, mockSubscribeOrderInfo.GetOrderInfo().OrderId).WaitOne();
            OrderInfo currentOrderState = mockSubscribeOrderInfo.GetOrderInfo();

            while (!currentOrderState.PendingType.Equals(PENDING_TYPE.COMPLETE))
            {
                Thread.Sleep(500);
                market.GetOrderInfo(myTestCoinType, mockSubscribeOrderInfo.GetOrderInfo().OrderId).WaitOne();
                currentOrderState = mockSubscribeOrderInfo.GetOrderInfo(); ;
            }
        }

        private bool OpenPositionTest(COIN_MARKET marketName, COIN_TYPE coinType, ORDER_SIDE side)
        {
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            OpationQuery query = new OpationQuery();

            COIN_MARKET myTestMarket = marketName;
            COIN_TYPE myTestCoinType = coinType;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(myTestMarket, query.CreateSettingValues(myTestMarket));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            market.GetOrderBook(myTestCoinType, false).WaitOne();
            OrderBook orderBook = mockSubscribeOrderbook.GetOrderBook();
            Assert.IsNotNull(orderBook);

            Order openOrder = this.CreateRealOrder(side, ORDER_DIRECTION.OPEN, orderBook, false);

            market.PlaceOrder(myTestCoinType, openOrder.OrderPrice, openOrder.Quantity, openOrder.Side, openOrder.Direction, openOrder.Type).WaitOne();
            OrderInfo placeOrderInfo = mockSubscribeOrderInfo.GetOrderInfo();
            Assert.IsNotNull(placeOrderInfo);

            market.GetOrderInfo(myTestCoinType, mockSubscribeOrderInfo.GetOrderInfo().OrderId).WaitOne();
            OrderInfo openOrderInfo = mockSubscribeOrderInfo.GetOrderInfo();
            Assert.IsNotNull(openOrderInfo);

            return true;
        }

        private bool ClosePositionTest(COIN_MARKET marketName, COIN_TYPE coinType, ORDER_SIDE side)
        {
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            OpationQuery query = new OpationQuery();

            COIN_MARKET myTestMarket = marketName;
            COIN_TYPE myTestCoinType = coinType;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(myTestMarket, query.CreateSettingValues(myTestMarket));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            ITrader trader = this.GetDefaultTrader();
            query.LoadBalanceAndPosition(trader);

            market.GetOrderBook(myTestCoinType, false).WaitOne();
            OrderBook orderBook = mockSubscribeOrderbook.GetOrderBook();
            Assert.IsNotNull(orderBook);

            Order closeOrder = this.CreateRealOrder(side, ORDER_DIRECTION.CLOSE, orderBook, false);
            Position pos = trader.GetPosition(myTestMarket, myTestCoinType);
            POSITION_SIDE posSide = side.Equals(ORDER_SIDE.buy) ? POSITION_SIDE.LONG : POSITION_SIDE.SHORT;

            market.PlaceOrder(myTestCoinType, closeOrder.OrderPrice, pos.GetQuantity(posSide), closeOrder.Side, closeOrder.Direction, closeOrder.Type).WaitOne();
            OrderInfo placeOrderInfo = mockSubscribeOrderInfo.GetOrderInfo();
            Assert.IsNotNull(placeOrderInfo);

            market.GetOrderInfo(myTestCoinType, mockSubscribeOrderInfo.GetOrderInfo().OrderId).WaitOne();
            OrderInfo closeOrderInfo = mockSubscribeOrderInfo.GetOrderInfo();
            Assert.IsNotNull(closeOrderInfo);

            return true;
        }

        private Order CreateRealOrder(ORDER_SIDE side, ORDER_DIRECTION direction, OrderBook orderBook, bool isSetPrice)
        {
            Order order = new Order();

            order.Side = side;
            order.Market = orderBook.Market;

            if (isSetPrice)
            {
                order.OrderPrice = 1800;
            }
            else
            {
                order.OrderPrice = side.Equals(ORDER_SIDE.buy) ? orderBook.DepthPrice(ORDERBOOK_SIDE.BID)[0] : orderBook.DepthPrice(ORDERBOOK_SIDE.ASK)[0];
            }

            order.Quantity = side.Equals(ORDER_SIDE.buy) ? orderBook.DepthQuantity(ORDERBOOK_SIDE.BID)[0] : orderBook.DepthQuantity(ORDERBOOK_SIDE.ASK)[0];
            order.Quantity = order.Quantity < 0.05 ? order.Quantity : 0.24;
            order.Symbol = orderBook.Symbol;
            order.OrderUnit = orderBook.OrderUnit;
            order.MinTradeValue = orderBook.MinTradeValue;
            order.Type = ORDER_TYPE.limit;
            order.Direction = direction;

            return order;
        }

        private ITrader GetDefaultTrader()
        {
            ITrader trader = new Trader();
            IList<Position> posList = new List<Position>();
            foreach (COIN_MARKET coinMarket in Enum.GetValues(typeof(COIN_MARKET)))
            {
                trader.RegisterMarket(MarketFactory.CreateMarket(coinMarket, new Settings()));

                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    Position pos = new Position(coinMarket, coinType);

                    pos.SetPrice(POSITION_SIDE.LONG, 0);
                    pos.SetQuantity(POSITION_SIDE.LONG, 0);
                    pos.SetPNL(POSITION_SIDE.LONG, 0);
                    pos.SetLeverage(POSITION_SIDE.LONG, 0);

                    pos.SetPrice(POSITION_SIDE.SHORT, 0);
                    pos.SetQuantity(POSITION_SIDE.SHORT, 0);
                    pos.SetPNL(POSITION_SIDE.SHORT, 0);
                    pos.SetLeverage(POSITION_SIDE.SHORT, 0);

                    pos.Total = 0;

                    trader.SetPosition(coinMarket, coinType, pos);
                }
            }

            return trader;
        }

        private void GetOrderInfoLifeCycleTest(COIN_MARKET marketName, COIN_TYPE coinType, string orderId)
        {
            OpationQuery query = new OpationQuery();
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();

            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(marketName, query.CreateSettingValues(marketName));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            AutoResetEvent orderInfoEvent =
                market.GetOrderInfo(coinType, orderId);

            orderInfoEvent.WaitOne();

            Assert.IsTrue(true);
        }

        //[TestMethod()]
        public void GetBalanceLifeCycleTest()
        {
            OpationQuery query = new OpationQuery();
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();

            COIN_MARKET myMarket = COIN_MARKET.FTX;
            COIN_TYPE myCoinType = COIN_TYPE.ETH;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(myMarket, query.CreateSettingValues(myMarket));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            AutoResetEvent orderInfoEvent =
                market.GetBalance();

            orderInfoEvent.WaitOne();

            Assert.IsTrue(true);
        }

        public void CancelInfoLifeCycleTest(COIN_MARKET marketName, string orderId)
        {
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            OpationQuery query = new OpationQuery();

            COIN_TYPE myCoinType = COIN_TYPE.ETH;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(marketName, query.CreateSettingValues(marketName));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            AutoResetEvent orderInfoEvent =
                market.CancelOrder(myCoinType, orderId);

            orderInfoEvent.WaitOne();

            Assert.IsTrue(true);
        }

        public void PlaceOrderLifeCycleTest()
        {
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            OpationQuery query = new OpationQuery();

            COIN_MARKET myMarket = COIN_MARKET.BINANCE;
            COIN_TYPE myCoinType = COIN_TYPE.ETH;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(myMarket, query.CreateSettingValues(myMarket));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            AutoResetEvent orderInfoEvent =
                market.PlaceOrder(COIN_TYPE.ETH, 2028.98, 1.600, ORDER_SIDE.sell, ORDER_DIRECTION.OPEN, ORDER_TYPE.limit);

            orderInfoEvent.WaitOne();

            Assert.IsTrue(true);
        }

        //[TestMethod()]
        public void GetOrderBookLifeCycleTest()
        {
            MockObserverOrderbook mockSubscribeOrderbook = new MockObserverOrderbook();
            MockObserverOrderInfo mockSubscribeOrderInfo = new MockObserverOrderInfo();
            OpationQuery query = new OpationQuery();

            COIN_MARKET myMarket = COIN_MARKET.HUOBI;
            COIN_TYPE myCoinType = COIN_TYPE.ETH;
            ICommunicator comm = new RESTAPICommunicator();

            IMarket market = MarketFactory.CreateMarket(myMarket, query.CreateSettingValues(myMarket));
            market.Subscribe(mockSubscribeOrderbook);
            market.Subscribe(mockSubscribeOrderInfo);
            market.RegisterCommuniator(comm);

            AutoResetEvent orderBookEvent =
                market.GetOrderBook(myCoinType, false);

            orderBookEvent.WaitOne();

            Assert.IsTrue(true);
        }
    }
}