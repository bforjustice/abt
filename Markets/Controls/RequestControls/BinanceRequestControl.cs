namespace Markets.Controls.RequestControls
{
    using Common;
    using Communication.Interfaces;
    using Configuration;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class BinanceRequestControl : RequestControlBase
    {
        public BinanceRequestControl(IRequestFactory factory)
            : base(factory)
        {
        }

        public override AutoResetEvent GetOrderbook(string symbol, int tId)
        {
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "symbol", symbol },
                    { "limit", "5" },
                };

            return base.GetOrderbook(symbol, parameters, tId);

            //            return new WebSocketRequest(REQUEST_TYPE.CONNECT_SERVER,
            //creator.CreateConnectionMessage(
            //settings,
            //WEBSOCKET_BASE_URL,
            //$"/stream?streams={symbol}@depth5@100ms",
            //parameters
            //))
            //            {
            //                DataSource = creator.CreatorType,
            //                Communicator = comm,
            //            };
        }

        public override AutoResetEvent GetBalance(int tId)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "apiKey", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                };

            return base.GetBalance(parameters, tId);
        }

        public override AutoResetEvent GetPosition(int tId)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "apiKey", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                };

            return base.GetPosition(parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        { "symbol", symbol },
                        { "leverage", leverage.ToString() },
                        { "apiKey", this.mySettings.API_KEY },
                        { "SecretKey", this.mySettings.SECRET_KEY },
                    };

            IRequest req = this.myReqFactory.CreateSetLeverageRequest(
                this.mySettings,
                parameters,
                this.FindCommunicator(DATA_SOURCE.REST),
                tId
                );

            Task<AutoResetEvent> task = new Task<AutoResetEvent>(req.Dispatch);
            task.Start();

            return task.Result;
        }

        public override AutoResetEvent PlaceOrder(string symbol,
        double price,
        double qty,
        ORDER_SIDE orderSide,
        ORDER_DIRECTION orderDirection,
        ORDER_TYPE orderType,
        int tId)
        {
            ORDER_SIDE mSide = orderSide;
            double mQty = qty;

            if (orderDirection.Equals(ORDER_DIRECTION.CLOSE) &&
                orderSide.Equals(ORDER_SIDE.buy))
            {
                mSide = ORDER_SIDE.sell;
            }
            else if (orderDirection.Equals(ORDER_DIRECTION.CLOSE) &&
                orderSide.Equals(ORDER_SIDE.sell))
            {
                mSide = ORDER_SIDE.buy;
            }

            mQty = Math.Abs(qty);

            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                            { "side", mSide.Equals(ORDER_SIDE.buy) ? "BUY" : "SELL" },
                            { "symbol", symbol },
                            { "type", orderType.Equals(ORDER_TYPE.limit) ? "LIMIT" : "MARKET" },
                            { "quantity", mQty.ToString("F8") },
                            { "price", price.ToString("F8") },
                            { "timeInForce", "GTC" },
                            { "apiKey", this.mySettings.API_KEY },
                            { "SecretKey", this.mySettings.SECRET_KEY },
                    };

            return base.PlaceOrder(symbol, price, qty, orderSide, orderDirection, orderType, parameters, tId);
        }

        public override AutoResetEvent CancelOrder(
            string symbol,
            string orderId,
            int tId
            )
        {
            Dictionary<string, string> parameters =
                 new Dictionary<string, string>()
                 {
                    { "symbol", symbol },
                    { "orderId", orderId},
                    { "apiKey", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                 };

            return base.CancelOrder(symbol, orderId, parameters, tId);
        }

        public override AutoResetEvent GetOrderInfo(
                string symbol,
                string orderId,
                int tId
            )
        {
            Dictionary<string, string> parameters =
                 new Dictionary<string, string>()
                 {
                    { "symbol", symbol },
                    { "orderId", orderId },
                    { "apiKey", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                 };

            return base.GetOrderInfo(symbol, orderId, parameters, tId);
        }

        public override AutoResetEvent GetTickers(int tId)
        {
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                };

            return base.GetTickers(parameters, tId);
        }
    }
}