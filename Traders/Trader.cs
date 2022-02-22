namespace Traders
{
    using Configuration;
    using DataModels;
    using Markets;
    using Markets.Converters;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Traders.Interfaces;

    public class Trader : IBalanceSubscriber, IPositionSubscriber, IOrderInfoSubscriber, ITrader
    {
        private object lockObj = new object();

        private IList<IMarket> myMarkets;

        private IDictionary<COIN_MARKET, Balance> myBalances;

        private IDictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>> myPositions;

        private IDictionary<string, OrderInfo> myOrderInfos;

        private IDictionary<IOrderPairSubscriber, int> myOrderPairSubscirbers;

        private IDictionary<int, WaitEvent> myOrderPairWaitEvents;

        public Trader()
        {
            this.myMarkets = new List<IMarket>();
            this.myBalances = new Dictionary<COIN_MARKET, Balance>();
            this.myPositions = new Dictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>>();
            this.myOrderInfos = new Dictionary<string, OrderInfo>();
            this.myOrderPairSubscirbers = new Dictionary<IOrderPairSubscriber, int>();
            this.myOrderPairWaitEvents = new Dictionary<int, WaitEvent>();
        }

        public void SetPosition(COIN_MARKET market, COIN_TYPE coinType, Position pos)
        {
            if (!this.myPositions.ContainsKey(market))
            {
                this.myPositions.Add(market, new Dictionary<COIN_TYPE, Position>());
            }

            this.myPositions[market][coinType] = pos;
            this.FindMarket(market).TranslatePositionValue(this.myPositions[market]);
        }

        public Position GetPosition(COIN_MARKET market, COIN_TYPE coinType)
        {
            IDictionary<COIN_TYPE, Position> tPos = null;

            if (this.myPositions.TryGetValue(market, out tPos))
            {
                Position position = null;
                if (tPos.TryGetValue(coinType, out position))
                {
                    return position;
                }
            }

            return null;
        }

        public IList<Position> GetAllPosition()
        {
            IList<Position> res = new List<Position>();

            foreach (COIN_MARKET market in this.myPositions.Keys)
            {
                foreach (COIN_TYPE coinType in this.myPositions[market].Keys)
                {
                    if (this.myPositions[market][coinType].STATE.Equals(REQUEST_STATE.NORMAL))
                    {
                        res.Add(this.myPositions[market][coinType]);
                    }
                }
            }

            return res;
        }

        public Balance GetBalance(COIN_MARKET market)
        {
            Balance balance = null;
            if (this.myBalances.TryGetValue(market, out balance))
            {
                return balance;
            }

            return null;
        }

        public IList<Balance> GetBalances()
        {
            return this.myBalances.Values.ToList();
        }

        public void SetBalance(COIN_MARKET market, Balance balance)
        {
            if (!this.myBalances.ContainsKey(market))
            {
                this.myBalances.Add(market, balance);
            }
            else
            {
                this.myBalances[market] = balance;
            }
        }

        public double TotalBalance_USDT()
        {
            double sum = 0;

            foreach (Balance bal in GetBalances())
            {
                sum += bal.TotalBalance_USDT();
            }

            return sum;
        }

        public void PublishBalance(Balance balance)
        {
            lock (lockObj)
            {
                if (!(balance.STATE.Equals(REQUEST_STATE.NORMAL)))
                {
                    return;
                }

                this.SetBalance(balance.Market, balance);
            }
        }

        public void PublishPosition(IList<Position> positions)
        {
            lock (lockObj)
            {
                foreach (Position pos in positions)
                {
                    this.SetPosition(pos.Market, pos.CoinType, pos);
                }
            }
        }

        public void RegisterMarket(IMarket market)
        {
            if (myMarkets.Contains(market))
            {
                return;
            }

            market.Subscribe(this as IBalanceSubscriber);
            market.Subscribe(this as IPositionSubscriber);
            myMarkets.Add(market);
        }

        public void UnregisterMarket(IMarket market)
        {
            if (myMarkets.Contains(market))
            {
                market.Unsubscribe(this as IBalanceSubscriber);
                market.Unsubscribe(this as IPositionSubscriber);
                myMarkets.Remove(market);
            }
        }

        public List<AutoResetEvent> RequestOrderInfo(IList<OrderInfo> orders)
        {
            List<AutoResetEvent> waitEvents = new List<AutoResetEvent>();

            foreach (OrderInfo info in orders)
            {
                IMarket market = this.FindMarket(info.Market);
                waitEvents.Add(Task.Run(() => this.openOrder(market, info.Identifier, info.OrderId, info.OrderbookId)).Result);
            }

            if (!this.myOrderPairWaitEvents.ContainsKey(orders.First().OrderbookId))
            {
                this.myOrderPairWaitEvents.Add(orders.First().OrderbookId, new WaitEvent(this, orders.First().OrderbookId, waitEvents));
            }
            else
            {
                this.myOrderPairWaitEvents[orders.First().OrderbookId] = new WaitEvent(this, orders.First().OrderbookId, waitEvents);
            }

            return waitEvents;
        }

        private AutoResetEvent coinOrder(IMarket market, string symbol, double price, double qty, ORDER_SIDE side, ORDER_DIRECTION direction, ORDER_TYPE type, int tId)
        {
            return market.PlaceOrder(symbol, price, qty, side, direction, type, tId);
        }

        private AutoResetEvent openOrder(IMarket market, string symbol, string orderId, int tId)
        {
            return market.GetOrderInfo(symbol, orderId, tId);
        }

        private IMarket FindMarket(COIN_MARKET market)
        {
            foreach (IMarket marketItem in this.myMarkets)
            {
                if (marketItem.GetMyMarketName().Equals(market))
                {
                    return marketItem;
                }
            }

            return MarketFactory.CreateEmptyMarket();
        }

        public void PublishOrderInfo(OrderInfo orderInfo)
        {
            if (!this.myOrderInfos.ContainsKey(orderInfo.OrderId))
            {
                this.myOrderInfos.Add(orderInfo.OrderId, orderInfo);
            }

            this.myOrderInfos[orderInfo.OrderId] = orderInfo;

            foreach (IOrderPairSubscriber item in this.myOrderPairSubscirbers.Keys)
            {
                if (orderInfo.OrderbookId.Equals(this.myOrderPairSubscirbers[item]))
                {
                    item.PublishOrderInfo(orderInfo);
                    this.myOrderPairWaitEvents[orderInfo.OrderbookId].Set();
                }
            }
        }

        public void CompelteEvent(int id)
        {
            this.myOrderPairWaitEvents.Remove(id);
        }

        public AutoResetEvent RequestCancelOrder(OrderInfo orderInfo)
        {
            IMarket market = this.FindMarket(orderInfo.Market);

            Task<AutoResetEvent> cancelOrder = Task.Run(() => market.CancelOrder(orderInfo.CoinType, orderInfo.OrderId));
            return cancelOrder.Result;
        }

        public AutoResetEvent RequestOrder(Order order)
        {
            return this.coinOrder(this.FindMarket(order.Market),
                order.Symbol,
                order.OrderPrice,
                order.Quantity,
                order.Side,
                order.Direction,
                order.Type,
                order.OrderbookId);
        }

        public MARKET_STATE GetMarketState(COIN_MARKET market)
        {
            return this.FindMarket(market).MarketState;
        }

        //public double GetMinTradeValue(COIN_MARKET market)
        //{
        //    return this.FindMarket(market).Settings.MinTradeValue;
        //}

        public void SetMarketState(COIN_MARKET market, MARKET_STATE state)
        {
            this.FindMarket(market).SetMarketState(state);
        }

        public void SubscribeOrderInfo(COIN_MARKET market, IOrderInfoSubscriber subscriber)
        {
            this.FindMarket(market).Subscribe(subscriber);
        }

        public void UnsubscribeOrderInfo(COIN_MARKET market, IOrderInfoSubscriber subscriber)
        {
            this.FindMarket(market).Unsubscribe(subscriber);
        }

        public void SubscribeOrderBook(COIN_MARKET market, IOrderbookSubscriber subscriber)
        {
            this.FindMarket(market).Subscribe(subscriber);
        }

        public void UnsubscribeOrderBook(COIN_MARKET market, IOrderbookSubscriber subscriber)
        {
            this.FindMarket(market).Unsubscribe(subscriber);
        }

        public AutoResetEvent RequestOrderInfo(OrderInfo orderInfo)
        {
            IMarket market = this.FindMarket(orderInfo.Market);
            return Task<AutoResetEvent>.Run(() => this.openOrder(market, orderInfo.Identifier, orderInfo.OrderId, orderInfo.OrderbookId)).Result;
        }

        public Position GetPosition(COIN_MARKET market, string symbol)
        {
            IDictionary<COIN_TYPE, Position> tPos = null;
            COIN_TYPE coinType = (COIN_TYPE)Enum.Parse(typeof(COIN_TYPE), CoinSymbolConverter.ConvertSymbolToCoinName(market, symbol));

            if (this.myPositions.TryGetValue(market, out tPos))
            {
                Position position = null;
                if (tPos.TryGetValue(coinType, out position))
                {
                    return position;
                }
            }

            return null;
        }

        public string SymbolExchange(COIN_MARKET srcMarket, COIN_MARKET dstMarket, string originSymbol)
        {
            string coinStr = CoinSymbolConverter.ConvertSymbolToCoinName(srcMarket, originSymbol);
            COIN_TYPE coinType = (COIN_TYPE)Enum.Parse(typeof(COIN_TYPE), coinStr);

            return CoinSymbolConverter.ConvertCoinSymbol(dstMarket, coinType);
        }

        public POSITION_TYPE GetPositionType(COIN_MARKET market)
        {
            return this.FindMarket(market).POSITION_TYPE;
        }

        public class WaitEvent
        {
            private List<AutoResetEvent> myEventList;

            private int count;

            private Trader myTrader;

            private int myId;

            public WaitEvent(Trader trader, int orderbookId, List<AutoResetEvent> list)
            {
                this.myTrader = trader;
                this.count = 0;
                this.myEventList = list;
                this.myId = orderbookId;
            }

            public void Set()
            {
                this.myEventList[count].Set();
                count++;

                if (count.Equals(this.myEventList.Count))
                {
                    this.myTrader.CompelteEvent(this.myId);
                }
            }
        }
    }
}