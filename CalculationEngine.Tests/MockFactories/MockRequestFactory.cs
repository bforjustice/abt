namespace CalculationEngine.Tests.MockFactories
{
    using CalculationEngine.Tests.Mocks;
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
            return new MockRequest(REQUEST_TYPE.CANCEL_ORDER,
                null,
                symbol,
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateConnectOrderbookStream(Settings settings, string symbol, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateGetBalanceRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new MockRequest(REQUEST_TYPE.GET_BALANCE,
                null,
                "Balance",
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateGetOrderbookRequest(Settings settings, string mCurrency, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new MockRequest(REQUEST_TYPE.ORDERBOOK,
                null,
                mCurrency,
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateGetOrderInfoRequest(Settings settings, string symbol, string orderId, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new MockRequest(REQUEST_TYPE.OPEN_ORDER,
                null,
                symbol,
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateGetPositionRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new MockRequest(REQUEST_TYPE.GET_POSITION,
                null,
                "Position",
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateGetTickerRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new MockRequest(REQUEST_TYPE.GET_TICKER,
                null,
                "Tickers",
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreatePlaceOrderRequest(Settings settings, string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new MockRequest(REQUEST_TYPE.PLACE_ORDER,
                null,
                symbol,
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateSetLeverageRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new MockRequest(REQUEST_TYPE.SET_LEVERAGE,
                null,
                "Leverage",
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateSubscribeOrderbookStream(Settings settings, string symbol, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new MockRequest(REQUEST_TYPE.SUBSCRIBE,
                null,
                symbol,
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }
    }
}