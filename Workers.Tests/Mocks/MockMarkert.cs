namespace Workers.Tests.Mocks
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class MockMarket : IMarket
    {
        public Settings Settings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int GMT { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string TEST_COINTYPE => this.myTestCoinName;

        public MARKET_STATE MarketState => throw new NotImplementedException();

        public POSITION_TYPE POSITION_TYPE { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private string myTestCoinName = string.Empty;

        public AutoResetEvent CancelOrder(COIN_TYPE coinType, string orderId)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent GetBalance()
        {
            throw new NotImplementedException();
        }

        public COIN_MARKET GetMyMarketName()
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent GetOrderBook(COIN_TYPE coinType, bool isHandlePending)
        {
            this.myTestCoinName = coinType.ToString();
            return new AutoResetEvent(false);
        }

        public AutoResetEvent GetOrderInfo(COIN_TYPE coinType, string orderId)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent GetPosition()
        {
            throw new NotImplementedException();
        }

        public void Notify(global::DataModels.OrderBook orderBook)
        {
            throw new NotImplementedException();
        }

        public void Notify(global::DataModels.Balance balnace)
        {
            throw new NotImplementedException();
        }

        public void Notify(IList<global::DataModels.Position> positions)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent PlaceOrder(COIN_TYPE coinType, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType)
        {
            throw new NotImplementedException();
        }

        public void RegisterOrderStopTime(IList<DateTime> stopTimes)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent SetLeverage(COIN_TYPE coinType, int leverage)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(IOrderbookSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(IBalanceSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(IPositionSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IOrderbookSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IBalanceSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IPositionSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void RegisterCommuniator(ICommunicator comm)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent PlaceOrder(string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(IOrderInfoSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IOrderInfoSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Notify(OrderInfo orderInfo)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent PlaceOrder(string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, int tId)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent GetOrderInfo(string symbol, string orderId, int tId)
        {
            throw new NotImplementedException();
        }

        public void SetMarketState(MARKET_STATE state)
        {
            throw new NotImplementedException();
        }

        public void ClearCommunicator()
        {
            throw new NotImplementedException();
        }

        public void TranslatePositionValue(IDictionary<COIN_TYPE, Position> positions)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent GetTickers()
        {
            throw new NotImplementedException();
        }

        public bool IsAvailableTradeCoin(string coinName)
        {
            throw new NotImplementedException();
        }

        public void PublishTickers(Tickers tickers)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(ITickerSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(ITickerSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Notify(Tickers tickers)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}