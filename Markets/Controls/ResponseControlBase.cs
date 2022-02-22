namespace Markets.Controls
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using LogTrace.Interfaces;
    using Markets.Converters;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;

    public abstract class ResponseControlBase : IJobSubscriber, IOrderbookPublisher, IBalancePublisher, IPositionPublisher, IOrderInfoPublisher, ITickerPublisher
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("ResponseControllLogger");

        protected string TAG = "ResponseControlBase";

        protected IList<IOrderbookSubscriber> orderBookPeers = new List<IOrderbookSubscriber>();

        protected IList<IOrderInfoSubscriber> orderInfoPeers = new List<IOrderInfoSubscriber>();

        protected IList<IBalanceSubscriber> balancePeers = new List<IBalanceSubscriber>();

        protected IList<IPositionSubscriber> positionPeers = new List<IPositionSubscriber>();

        protected IList<ITickerSubscriber> tickerPeers = new List<ITickerSubscriber>();

        protected Settings mySettings;

        private IList<ICommunicator> comms = new List<ICommunicator>();

        public void RegisterCommunicator(ICommunicator comm)
        {
            if (comms.Contains(comm))
            {
                return;
            }

            comms.Add(comm);

            comm.Subscribe(this);
        }

        public void UnegisterCommunicator(ICommunicator comm)
        {
            if (comms.Contains(comm))
            {
                comm.Unsubscribe(this);
                comms.Remove(comm);
            }
        }

        public void ClearCommunicator()
        {
            foreach (ICommunicator comm in this.comms)
            {
                comm.Unsubscribe(this);
            }

            comms.Clear();
        }

        public Settings Settings { get { return this.mySettings; } set { this.mySettings = value; } }

        public virtual void PublishJob(APIResult result)
        {
            try
            {
                switch (result.Method)
                {
                    case REQUEST_TYPE.ORDERBOOK:
                        this.Notify(this.ParseOrderBook(this.Settings, result));
                        break;

                    case REQUEST_TYPE.GET_BALANCE:
                        this.Notify(this.ParseBalance(this.Settings, result));
                        break;

                    case REQUEST_TYPE.GET_POSITION:
                        this.Notify(this.ParsePosition(this.Settings, result));
                        break;

                    case REQUEST_TYPE.OPEN_ORDER:
                        this.Notify(this.ParseOrderInfo(this.Settings, result));
                        break;

                    case REQUEST_TYPE.SET_LEVERAGE:
                        this.ParseLeverage(this.Settings, result);
                        break;

                    case REQUEST_TYPE.PLACE_ORDER:
                        this.Notify(this.ParsePlaceOrder(this.Settings, result));
                        break;

                    case REQUEST_TYPE.CANCEL_ORDER:
                        this.Notify(this.ParseCancelOrder(this.Settings, result));
                        break;

                    case REQUEST_TYPE.GET_TICKER:
                        this.Notify(this.ParseTickers(this.Settings, result));
                        break;
                }
            }
            catch (Exception e)
            {
                myLogger.Error($"Error : Result Msg : {result.Result}\n Method : {result.Method.ToString()}");
            }
            finally
            {
                result.DoneEvent.Set();
            }
        }

        public void Subscribe(IOrderInfoSubscriber subscriber)
        {
            if (this.orderInfoPeers.Contains(subscriber))
            {
                return;
            }

            this.orderInfoPeers.Add(subscriber);
        }

        public void Unsubscribe(IOrderInfoSubscriber subscriber)
        {
            if (this.orderInfoPeers.Contains(subscriber))
            {
                this.orderInfoPeers.Remove(subscriber);
            }
        }

        public void Notify(OrderInfo orderInfo)
        {
            foreach (IOrderInfoSubscriber subscriber in this.orderInfoPeers)
            {
                subscriber.PublishOrderInfo(orderInfo);
            }
        }

        public void Subscribe(IOrderbookSubscriber subscriber)
        {
            if (this.orderBookPeers.Contains(subscriber))
            {
                return;
            }

            this.orderBookPeers.Add(subscriber);
        }

        public void Unsubscribe(IOrderbookSubscriber subscriber)
        {
            if (this.orderBookPeers.Contains(subscriber))
            {
                this.orderBookPeers.Remove(subscriber);
            }
        }

        public void Notify(OrderBook orderBook)
        {
            foreach (IOrderbookSubscriber subscriber in this.orderBookPeers)
            {
                subscriber.PublishOrderbook(orderBook);
            }
        }

        public void Subscribe(IBalanceSubscriber subscriber)
        {
            if (this.balancePeers.Contains(subscriber))
            {
                return;
            }

            this.balancePeers.Add(subscriber);
        }

        public void Unsubscribe(IBalanceSubscriber subscriber)
        {
            if (this.balancePeers.Contains(subscriber))
            {
                this.balancePeers.Remove(subscriber);
            }
        }

        public void Notify(Balance balnace)
        {
            foreach (IBalanceSubscriber subscriber in this.balancePeers)
            {
                subscriber.PublishBalance(balnace);
            }
        }

        public void Subscribe(IPositionSubscriber subscriber)
        {
            if (this.positionPeers.Contains(subscriber))
            {
                return;
            }

            this.positionPeers.Add(subscriber);
        }

        public void Unsubscribe(IPositionSubscriber subscriber)
        {
            if (this.positionPeers.Contains(subscriber))
            {
                this.positionPeers.Remove(subscriber);
            }
        }

        public void Notify(IList<Position> positions)
        {
            foreach (IPositionSubscriber subscriber in this.positionPeers)
            {
                subscriber.PublishPosition(positions);
            }
        }

        public abstract OrderBook ParseOrderBookByWebSocket(Settings settings, APIResult result);

        public abstract OrderBook ParseOrderBook(Settings settings, APIResult result);

        public abstract Balance ParseBalance(Settings settings, APIResult result);

        public abstract IList<Position> ParsePosition(Settings settings, APIResult result);

        public abstract OrderInfo ParseOrderInfo(Settings settings, APIResult result);

        public abstract string ParseLeverage(Settings settings, APIResult result);

        public abstract OrderInfo ParsePlaceOrder(Settings settings, APIResult result);

        public abstract OrderInfo ParseCancelOrder(Settings settings, APIResult result);

        public abstract Tickers ParseTickers(Settings settings, APIResult result);

        public void Subscribe(ITickerSubscriber subscriber)
        {
            if (this.tickerPeers.Contains(subscriber))
            {
                return;
            }

            this.tickerPeers.Add(subscriber);
        }

        public void Unsubscribe(ITickerSubscriber subscriber)
        {
            if (this.tickerPeers.Contains(subscriber))
            {
                this.tickerPeers.Remove(subscriber);
            }
        }

        public void Notify(Tickers tickers)
        {
            foreach (ITickerSubscriber subscriber in this.tickerPeers)
            {
                subscriber.PublishTickers(tickers);
            }
        }

        protected void ErrorLog(string marketName, string apiResult)
        {
            myLogger.Error($"Crash :: Market : {marketName} : apiResult : \n {apiResult}");
        }

        protected IList<Position> GetDefaultPosition(COIN_MARKET coinMarket)
        {
            IList<Position> posList = new List<Position>();
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

                posList.Add(pos);
            }

            return posList;
        }

        protected OrderBook GenerateOrderBookWithParam(COIN_MARKET marketName, APIResult apiResult, Settings settings)
        {
            OrderBook orderBook = new OrderBook();
            orderBook.Market = marketName;
            orderBook.Symbol = apiResult.Identifier;
            orderBook.ElspTime = apiResult.ElspTime;
            orderBook.TakerFee = this.mySettings.TakerFee;
            orderBook.Bound = this.mySettings.Bound;

            COIN_TYPE coinType = (COIN_TYPE)Enum.Parse(typeof(COIN_TYPE),
                CoinSymbolConverter.ConvertSymbolToCoinName(orderBook.Market, orderBook.Symbol));

            orderBook.CoinType = coinType;
            orderBook.DecimalLength = this.mySettings.GetDecimalLength(coinType);
            orderBook.OrderUnit = this.mySettings.GetOrderUnit(coinType);
            orderBook.MinTradeValue = this.mySettings.GetMinTradeValue(coinType);
            orderBook.PriceDiffThreshold = this.mySettings.GetPriceDiffThreshold(coinType);
            orderBook.MarginThreshold = this.mySettings.GetMarginThreshold(coinType);

            return orderBook;
        }

        protected bool CheckResultValidity(APIResult item)
        {
            if (!item.STATE.Equals(REQUEST_STATE.NORMAL))
            {
                return true;
            }

            return false;
        }

        protected OrderInfo ConvertAPIResultToOrderInfo(COIN_MARKET market, APIResult result)
        {
            OrderInfo info = new OrderInfo();
            info.Identifier = result.Identifier;
            info.Market = market;
            info.Type = result.Method;
            info.OrderbookId = result.Tid;
            info.CoinType = (COIN_TYPE)Enum.Parse(typeof(COIN_TYPE),
                    CoinSymbolConverter.ConvertSymbolToCoinName(info.Market, result.Identifier));

            return info;
        }
    }
}