namespace Markets
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using Markets.Controls;
    using Markets.Converters;
    using Markets.Interfaces;
    using Markets.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using TidGenerators;

    public class Market : IMarket
    {
        public Market(COIN_MARKET market, Settings settings, RequestControlBase reqCtrl, ResponseControlBase resCtrl)
        {
            this.myMarketName = market;

            this.myRequester = reqCtrl;
            this.myResponser = resCtrl;

            this.myTickers = new Tickers();
            this.myResponser.Subscribe(this);

            this.Settings = settings;

            Thread t = new Thread(new ThreadStart(observeStopMarket));
            t.Start();
        }

        public Settings Settings
        {
            get
            {
                return this.mySettings;
            }
            set
            {
                this.mySettings = value;
                this.myRequester.Settings = value;
                this.myResponser.Settings = value;
            }
        }

        public POSITION_TYPE POSITION_TYPE
        {
            get
            {
                return this.mySettings.POSITION_TYPE;
            }
            set
            {
                this.mySettings.POSITION_TYPE = value;
            }
        }

        protected Settings mySettings;
        protected DataManager myDataManager;
        protected COIN_MARKET myMarketName;
        protected MARKET_STATE myMarketState;

        protected RequestControlBase myRequester;
        protected ResponseControlBase myResponser;

        protected Tickers myTickers;

        private IList<DateTime> stopTimers = new List<DateTime>();

        public MARKET_STATE MarketState
        {
            get { return this.myMarketState; }
        }

        public int GMT { get; set; }

        public void RegisterOrderStopTime(IList<DateTime> stopTimes)
        {
            this.stopTimers = stopTimes;
        }

        public void Notify(OrderBook orderBook)
        {
            this.myResponser.Notify(orderBook);
            return;
        }

        public void Notify(Balance balnace)
        {
            this.myResponser.Notify(balnace);
            return;
        }

        public void Notify(IList<Position> positions)
        {
            this.myResponser.Notify(positions);
            return;
        }

        public void Subscribe(IOrderbookSubscriber subscriber)
        {
            this.myResponser.Subscribe(subscriber);
            return;
        }

        public void Subscribe(IBalanceSubscriber subscriber)
        {
            this.myResponser.Subscribe(subscriber);
            return;
        }

        public void Subscribe(IPositionSubscriber subscriber)
        {
            this.myResponser.Subscribe(subscriber);
            return;
        }

        public void Subscribe(ITickerSubscriber subscriber)
        {
            this.myResponser.Subscribe(subscriber);
            return;
        }

        public void Unsubscribe(IOrderbookSubscriber subscriber)
        {
            this.myResponser.Unsubscribe(subscriber);
            return;
        }

        public void Unsubscribe(IBalanceSubscriber subscriber)
        {
            this.myResponser.Unsubscribe(subscriber);
            return;
        }

        public void Unsubscribe(IPositionSubscriber subscriber)
        {
            this.myResponser.Unsubscribe(subscriber);
            return;
        }

        public void Unsubscribe(ITickerSubscriber subscriber)
        {
            this.myResponser.Unsubscribe(subscriber);
            return;
        }

        public void Initialize()
        {
            this.GetTickers().WaitOne();
        }

        public AutoResetEvent CancelOrder(COIN_TYPE coinType, string orderId)
        {
            return this.myRequester.CancelOrder(
                CoinSymbolConverter.ConvertCoinSymbol(this.GetMyMarketName(), coinType),
                orderId,
                TidGenerator.GenerateTransactionId()
                );
        }

        public AutoResetEvent GetBalance()
        {
            return this.myRequester.GetBalance(TidGenerator.GenerateTransactionId());
        }

        public COIN_MARKET GetMyMarketName()
        {
            return this.myMarketName;
        }

        public AutoResetEvent GetOrderBook(COIN_TYPE coinType, bool isHandlePending)
        {
            return this.myRequester.GetOrderbook(
                CoinSymbolConverter.ConvertCoinSymbol(this.GetMyMarketName(), coinType),
                TidGenerator.GenerateTransactionId()
                );
        }

        public AutoResetEvent GetOrderInfo(COIN_TYPE coinType, string orderId)
        {
            return this.myRequester.GetOrderInfo(
                CoinSymbolConverter.ConvertCoinSymbol(this.GetMyMarketName(), coinType),
                orderId,
                TidGenerator.GenerateTransactionId()
                );
        }

        public AutoResetEvent GetOrderInfo(string symbol, string orderId, int tId)
        {
            return this.myRequester.GetOrderInfo(
                symbol,
                orderId,
                tId
                );
        }

        public AutoResetEvent GetPosition()
        {
            return this.myRequester.GetPosition(TidGenerator.GenerateTransactionId());
        }

        public AutoResetEvent PlaceOrder(COIN_TYPE coinType, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType)
        {
            return this.PlaceOrder(
                CoinSymbolConverter.ConvertCoinSymbol(this.GetMyMarketName(), coinType),
                price,
                qty,
                orderSide,
                orderDirection,
                orderType,
                TidGenerator.GenerateTransactionId()
                );
        }

        public AutoResetEvent PlaceOrder(string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, int tId)
        {
            return this.myRequester.PlaceOrder(
                symbol,
                price,
                qty,
                orderSide,
                orderDirection,
                orderType,
                tId);
        }

        public AutoResetEvent SetLeverage(COIN_TYPE coinType, int leverage)
        {
            return this.myRequester.SetLeverage(
                CoinSymbolConverter.ConvertCoinSymbol(this.GetMyMarketName(), coinType),
                leverage,
                TidGenerator.GenerateTransactionId()
                );
        }

        private void observeStopMarket()
        {
            while (true)
            {
                DateTime nowDateTime = DateTime.UtcNow.AddHours(GMT);

                foreach (DateTime stopTime in this.stopTimers)
                {
                    if (nowDateTime.Hour.Equals(stopTime.Hour) &&
                        nowDateTime.Minute.Equals(stopTime.Minute) &&
                        nowDateTime.Second > stopTime.Second)
                    {
                        this.SetMarketState(MARKET_STATE.SATTLE_FUNDING_FEE);
                        Thread.Sleep(15000);
                        this.SetMarketState(MARKET_STATE.NORMAL);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void RegisterCommuniator(ICommunicator comm)
        {
            this.myRequester?.RegisterCommunicator(comm);
            this.myResponser?.RegisterCommunicator(comm);
        }

        public void Notify(OrderInfo orderInfo)
        {
            return;
        }

        public void Subscribe(IOrderInfoSubscriber subscriber)
        {
            this.myResponser.Subscribe(subscriber);
        }

        public void Unsubscribe(IOrderInfoSubscriber subscriber)
        {
            this.myResponser.Unsubscribe(subscriber);
        }

        public void SetMarketState(MARKET_STATE state)
        {
            this.myMarketState = state;
        }

        public void ClearCommunicator()
        {
            this.myRequester?.ClearCommunicator();
            this.myResponser?.ClearCommunicator();
        }

        public void TranslatePositionValue(IDictionary<COIN_TYPE, Position> positions)
        {
            this.myRequester?.CalcTotalPosition(positions);
        }

        public AutoResetEvent GetTickers()
        {
            return this.myRequester.GetTickers(TidGenerator.GenerateTransactionId());
        }

        public bool IsAvailableTradeCoin(string coinName)
        {
            return this.myTickers.GetCoin(coinName) != null;
        }

        public void PublishTickers(Tickers tickers)
        {
            this.myTickers = tickers;
        }

        public void Notify(Tickers tickers)
        {
            this.myResponser.Notify(tickers);
        }
    }
}