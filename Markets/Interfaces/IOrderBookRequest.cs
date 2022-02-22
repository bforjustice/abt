namespace Markets.Interfaces
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using System.Collections.Generic;
    using System.Threading;

    public interface IOrderBookRequest
    {
        AutoResetEvent GetOrderBook(Settings settings, string mCurrency, bool isHandlePending);

        AutoResetEvent GetBalance(Settings settings, Dictionary<string, string> parameters);

        AutoResetEvent GetPosition(Settings settings, Dictionary<string, string> parameters);

        AutoResetEvent SetLeverage(Settings settings, string symbol, int leverage, Dictionary<string, string> parameters);

        AutoResetEvent PlaceOrder(Settings settings, string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, Dictionary<string, string> parameters);

        AutoResetEvent CancelOrder(Settings settings, string symbol, string orderId, Dictionary<string, string> parameters);

        AutoResetEvent GetOrderInfo(Settings settings, string symbol, string orderId, Dictionary<string, string> parameters);

        void Subscribe(IJobSubscriber subscriber);

        void Unsubscribe(IJobSubscriber subscriber);
    }
}