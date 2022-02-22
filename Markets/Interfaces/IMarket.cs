namespace Markets.Interfaces
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public interface IMarket : IOrderbookPublisher, IBalancePublisher, IPositionPublisher, IOrderInfoPublisher, ITickerSubscriber, ITickerPublisher
    {
        COIN_MARKET GetMyMarketName();

        Settings Settings { get; set; }

        AutoResetEvent GetBalance();

        AutoResetEvent GetPosition();

        AutoResetEvent GetTickers();

        AutoResetEvent GetOrderBook(COIN_TYPE coinType, bool isHandlePending);

        AutoResetEvent PlaceOrder(COIN_TYPE coinType, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType);

        AutoResetEvent PlaceOrder(string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, int tId);

        AutoResetEvent GetOrderInfo(COIN_TYPE coinType, string orderId);

        AutoResetEvent GetOrderInfo(string symbol, string orderId, int tId);

        AutoResetEvent CancelOrder(COIN_TYPE coinType, string orderId);

        AutoResetEvent SetLeverage(COIN_TYPE coinType, int leverage);

        void TranslatePositionValue(IDictionary<COIN_TYPE, Position> positions);

        int GMT { get; set; }

        void RegisterOrderStopTime(IList<DateTime> stopTimes);

        void RegisterCommuniator(ICommunicator comm);

        void SetMarketState(MARKET_STATE state);

        MARKET_STATE MarketState { get; }

        void ClearCommunicator();

        bool IsAvailableTradeCoin(string coinName);

        void Initialize();

        POSITION_TYPE POSITION_TYPE { get; set; }
    }
}