namespace Markets.Controls.RequestControls
{
    using Common;
    using Configuration;
    using Markets.Interfaces;
    using System.Collections.Generic;
    using System.Threading;

    public class ZBGRequestControl : RequestControlBase
    {
        public ZBGRequestControl(IRequestFactory factory)
            : base(factory)
        {
        }

        public override AutoResetEvent GetBalance(int tId)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "timestamp", TimeManager.UtcTimeMS().ToString() },
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
                    { "timestamp", TimeManager.UtcTimeMS().ToString() },
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
                    { "size", "5" },
                };

            return base.GetOrderbook(symbol, parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            return new AutoResetEvent(false);
        }

        public override AutoResetEvent PlaceOrder(string symbol,
            double price,
            double qty,
            ORDER_SIDE orderSide,
            ORDER_DIRECTION orderDirection,
            ORDER_TYPE orderType,
            int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        { "symbol", symbol },
                        { "price" , price.ToString() },
                        { "quantity", qty.ToString() },
                        { "side",(orderSide.Equals(ORDER_SIDE.buy)) ? "1" : "-1" },
                        { "orderType",(orderType.Equals(ORDER_TYPE.limit)) ? "1" : "3" },
                        { "positionEffect",(orderType.Equals(ORDER_DIRECTION.OPEN)) ? "1" : "2" },
                        { "marginType", "1" }, // Cross
                        { "marginRate", "0" }, // Cross
                        { "timestamp", TimeManager.UtcTimeMS().ToString() },
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
                    { "symbol", symbol},
                    { "orderId", orderId},
                        { "timestamp", TimeManager.UtcTimeMS().ToString() },
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
                        { "timestamp", TimeManager.UtcTimeMS().ToString() },
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