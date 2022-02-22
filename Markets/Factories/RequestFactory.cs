namespace Markets.Factories
{
    using Communication.Interfaces;
    using Communication.Models;
    using Configuration;
    using DataModels;
    using Markets.Interfaces;
    using MessageBuilders;
    using MessageBuilders.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class RequestFactory : IRequestFactory
    {
        //private IDictionary<string, IRequestCreator> myRESTRequesters;
        private IDictionary<string, string> myRESTRequesters;

        public RequestFactory(string filePath)
        {
            //this.myRESTRequesters = new Dictionary<string, IRequestCreator>()
            this.myRESTRequesters = new Dictionary<string, string>()
            {
                { "Orderbook", filePath },
                { "Order",  filePath },
                { "Balance",  filePath },
                { "Position",  filePath },
                { "Leverage",  filePath },
                { "CancelOrder",  filePath },
                { "OrderInfo",  filePath },
                { "Tickers",  filePath },
            };
        }

        public IRequest CreateCancelOrderRequest(Settings settings, string symbol, string orderId, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new RESTHttpRequest(REQUEST_TYPE.CANCEL_ORDER,
                this.GetRequest("CancelOrder").Create(parameters),
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
            return new RESTHttpRequest(REQUEST_TYPE.GET_BALANCE,
                this.GetRequest("Balance").Create(parameters),
                "Balance",
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateGetOrderbookRequest(Settings settings, string mCurrency, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new RESTRequest(REQUEST_TYPE.ORDERBOOK,
                this.GetRequest("Orderbook").Create(parameters),
                mCurrency,
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateGetOrderInfoRequest(Settings settings, string symbol, string orderId, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new RESTHttpRequest(REQUEST_TYPE.OPEN_ORDER,
                this.GetRequest("OrderInfo").Create(parameters),
                symbol,
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateGetPositionRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new RESTHttpRequest(REQUEST_TYPE.GET_POSITION,
                this.GetRequest("Position").Create(parameters),
                "Position",
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateGetTickerRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new RESTHttpRequest(REQUEST_TYPE.GET_TICKER,
                this.GetRequest("Tickers").Create(parameters),
                "Tickers",
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreatePlaceOrderRequest(Settings settings, string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new RESTHttpRequest(REQUEST_TYPE.PLACE_ORDER,
                this.GetRequest("Order").Create(parameters),
                symbol,
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateSetLeverageRequest(Settings settings, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            return new RESTHttpRequest(REQUEST_TYPE.SET_LEVERAGE,
                this.GetRequest("Leverage").Create(parameters),
                "Leverage",
                tId)
            {
                DataSource = comm.COMMUNICATOR_TYPE,
                Communicator = comm,
            };
        }

        public IRequest CreateSubscribeOrderbookStream(Settings settings, string symbol, IDictionary<string, string> parameters, ICommunicator comm, int tId)
        {
            throw new NotImplementedException();
        }

        private IRequestCreator GetRequest(string key)
        {
            //IRequestCreator requester = null;
            string requester = null;
            if (this.myRESTRequesters.TryGetValue(key, out requester))
            {
                return MessageComponentBuilder.CreateRequester(key, this.myRESTRequesters[key]);
            }

            throw new KeyNotFoundException("No have Reuqester");
        }
    }
}