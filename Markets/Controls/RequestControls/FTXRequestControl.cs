namespace Markets.Controls.RequestControls
{
    using Common;
    using Configuration;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class FTXRequestControl : RequestControlBase
    {
        public FTXRequestControl(IRequestFactory factory)
            : base(factory)
        {
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

        public override AutoResetEvent GetOrderbook(string symbol, int tId)
        {
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "symbol", symbol },
                    { "depth", "6" },
                };

            return base.GetOrderbook(symbol, parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        //{ "leverage", leverage.ToString() },
                        { "leverage", "10" },
                        { "apiKey", this.mySettings.API_KEY },
                        { "SecretKey", this.mySettings.SECRET_KEY },
                    };

            return base.SetLeverage(symbol, leverage, parameters, tId);
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
            double mQty = Math.Abs(qty);
            bool isReduceOnly = false;

            if (orderDirection.Equals(ORDER_DIRECTION.CLOSE) &&
                orderSide.Equals(ORDER_SIDE.buy))
            {
                mSide = ORDER_SIDE.sell;
                isReduceOnly = true;
            }
            else if (orderDirection.Equals(ORDER_DIRECTION.CLOSE) &&
                orderSide.Equals(ORDER_SIDE.sell))
            {
                mSide = ORDER_SIDE.buy;
                isReduceOnly = true;
            }

            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "market", symbol},
                    { "side", mSide.Equals(ORDER_SIDE.buy) ? "buy" : "sell" },
                    { "price", price.ToString() },
                    { "type", orderType.Equals(ORDER_TYPE.limit) ? "limit" : "market" },
                    { "size", mQty.ToString() },
                    { "reduceOnly", isReduceOnly.ToString().ToLower() },
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
                    { "order_id", orderId},
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
                    { "order_id", orderId},
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