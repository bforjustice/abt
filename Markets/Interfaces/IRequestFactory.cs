namespace Markets.Interfaces
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using System.Collections.Generic;

    public interface IRequestFactory
    {
        IRequest CreateGetBalanceRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreateGetPositionRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreateGetOrderbookRequest(Settings settings, string mCurrency, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreateGetTickerRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreateSetLeverageRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreatePlaceOrderRequest(Settings settings, string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreateCancelOrderRequest(Settings settings, string symbol, string orderId, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreateGetOrderInfoRequest(Settings settings, string symbol, string orderId, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreateConnectOrderbookStream(Settings settings, string symbol, IDictionary<string, string> parameters, ICommunicator comm, int tId);

        IRequest CreateSubscribeOrderbookStream(Settings settings, string symbol, IDictionary<string, string> parameters, ICommunicator comm, int tId);
    }
}