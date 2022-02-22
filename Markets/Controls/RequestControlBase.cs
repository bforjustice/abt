namespace Markets.Controls
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class RequestControlBase
    {
        private IList<ICommunicator> comms = new List<ICommunicator>();

        protected IRequestFactory myReqFactory;

        public RequestControlBase(IRequestFactory factory)
        {
            this.myReqFactory = factory;
        }

        public void RegisterCommunicator(ICommunicator comm)
        {
            if (comms.Contains(comm))
            {
                return;
            }

            comms.Add(comm);
        }

        public void UnegisterCommunicator(ICommunicator comm)
        {
            if (comms.Contains(comm))
            {
                comms.Remove(comm);
            }
        }

        public void ClearCommunicator()
        {
            this.comms.Clear();
        }

        protected Settings mySettings;

        public Settings Settings { get { return this.mySettings; } set { this.mySettings = value; } }

        public abstract AutoResetEvent GetBalance(int tId);

        public abstract AutoResetEvent GetPosition(int tId);

        public abstract AutoResetEvent GetTickers(int tId);

        public abstract AutoResetEvent GetOrderbook(string symbol, int tId);

        public abstract AutoResetEvent PlaceOrder(string symbol,
            double price,
            double qty,
            ORDER_SIDE orderSide,
            ORDER_DIRECTION orderDirection,
            ORDER_TYPE orderType,
            int tId);

        public abstract AutoResetEvent CancelOrder(
            string symbol,
            string orderId,
            int tId
            );

        public abstract AutoResetEvent SetLeverage(string symbol, int leverage, int tId);

        public abstract AutoResetEvent GetOrderInfo(string symbol, string orderId, int tId);

        public virtual AutoResetEvent SubscribeOrderbook(
            string symbol,
            int tId
            )
        {
            IRequest req = this.myReqFactory.CreateConnectOrderbookStream(
                this.mySettings,
                symbol,
                new Dictionary<string, string>(),
                this.FindCommunicator(DATA_SOURCE.WEBSOCKET),
                tId
                );

            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }

            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        protected ICommunicator FindCommunicator(DATA_SOURCE source)
        {
            foreach (ICommunicator comm in comms)
            {
                if (comm.COMMUNICATOR_TYPE.Equals(source))
                {
                    return comm;
                }
            }

            throw new NotImplementedException($"Failed to Find commucation type : {source.ToString()}");
        }

        protected virtual AutoResetEvent GetBalance(IDictionary<string, string> parameters, int tId)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            IRequest req = this.myReqFactory.CreateGetBalanceRequest(
                this.mySettings,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId
                );

            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }

            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        protected virtual AutoResetEvent GetPosition(IDictionary<string, string> parameters, int tId)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            IRequest req = this.myReqFactory.CreateGetPositionRequest(
                this.mySettings,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId
                );

            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }

            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        public virtual AutoResetEvent PlaceOrder(string symbol,
            double price,
            double qty,
            ORDER_SIDE orderSide,
            ORDER_DIRECTION orderDirection,
            ORDER_TYPE orderType,
            IDictionary<string, string> parameters,
            int tId)
        {
            IRequest req = this.myReqFactory.CreatePlaceOrderRequest(
                this.mySettings,
                symbol,
                price,
                qty,
                orderSide,
                orderDirection,
                orderType,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId
                );

            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }

            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        public virtual AutoResetEvent CancelOrder(
            string symbol,
            string orderId,
            IDictionary<string, string> parameters,
            int tId)
        {
            IRequest req = this.myReqFactory.CreateCancelOrderRequest(
                this.mySettings,
                symbol,
                orderId,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId
                );

            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }

            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        public virtual AutoResetEvent SetLeverage(
            string symbol,
            int leverage,
            IDictionary<string, string> parameters,
            int tId)
        {
            IRequest req = this.myReqFactory.CreateSetLeverageRequest(
                this.mySettings,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId
                );

            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }

            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        public virtual AutoResetEvent GetOrderInfo(
                string symbol,
                string orderId,
                IDictionary<string, string> parameters,
                int tId
                )
        {
            IRequest req = this.myReqFactory.CreateGetOrderInfoRequest(
                this.mySettings,
                symbol,
                orderId,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId
                );
            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }
            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        public virtual void CalcTotalPosition(IDictionary<COIN_TYPE, Position> positions)
        {
            foreach (Position position in positions.Values)
            {
                double sl = position.GetPrice(POSITION_SIDE.SHORT) * position.GetQuantity(POSITION_SIDE.SHORT);
                double ll = position.GetPrice(POSITION_SIDE.LONG) * position.GetQuantity(POSITION_SIDE.LONG);
                position.Total = sl + ll;
            }
        }

        protected virtual AutoResetEvent GetOrderbook(string symbol, IDictionary<string, string> parameters, int tId)
        {
            IRequest req = this.myReqFactory.CreateGetOrderbookRequest(
                this.mySettings,
                symbol,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId);

            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();
            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }
            return task.Result;
        }

        protected virtual AutoResetEvent GetTickers(IDictionary<string, string> parameters, int tId)
        {
            IRequest req = this.myReqFactory.CreateGetTickerRequest(
                this.mySettings,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId);
            if (req.Equals(null))
            {
                return new AutoResetEvent(true);
            }
            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        //{
        //    IRequest req = this.myReqFactory.CreateGetOrderbookRequest(
        //        this.mySettings,
        //        symbol,
        //        new Dictionary<string, string>(),
        //        this.FindCommunicator(DATA_SOURCE.REST),
        //        tId);

        //    Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
        //    task.Start();

        //    return task.Result;

        //if (!this.FindCommunicator(DATA_SOURCE.WEBSOCKET).IsConnected)
        //{
        //    this.FindCommunicator(DATA_SOURCE.WEBSOCKET).ConnectStream
        //        (
        //            this.myReqFactory.CreateConnectOrderbookStream(
        //                this.mySettings,
        //                symbol,
        //                new Dictionary<string, string>(),
        //                this.myMsgCreator,
        //                this.FindCommunicator(DATA_SOURCE.WEBSOCKET))
        //                .GetRequest() as string
        //        );

        //    IRequest req = this.myReqFactory.CreateSubscribeOrderbookStream(
        //        this.mySettings,
        //        symbol,
        //        new Dictionary<string, string>(),
        //        this.myMsgCreator,
        //        this.FindCommunicator(DATA_SOURCE.WEBSOCKET)
        //        );

        //    Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
        //    task.Start();

        //    return task.Result;
        //}

        //return new AutoResetEvent(true);
        //}
    }
}