namespace Traders.Tests.Mocks
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;

    public class MockRequestFactory : IRequestFactory
    {
        public IRequest CreateCancelOrderRequest(Settings settings, string symbol, string orderId, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateConnectOrderbookStream(Settings settings, string symbol, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateGetBalanceRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateGetOrderbookRequest(Settings settings, string mCurrency, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateGetOrderInfoRequest(Settings settings, string symbol, string orderId, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateGetPositionRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateGetTickerRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreatePlaceOrderRequest(Settings settings, string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateSetLeverageRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateSubscribeOrderbookStream(Settings settings, string symbol, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }
    }
}