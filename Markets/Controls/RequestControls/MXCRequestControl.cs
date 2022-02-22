namespace Markets.Controls.RequestControls
{
    using Common;
    using Configuration;
    using Markets.Interfaces;
    using System.Collections.Generic;
    using System.Threading;

    public class MXCRequestControl : RequestControlBase
    {
        public MXCRequestControl(IRequestFactory factory)
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
                    { "api_key", this.mySettings.API_KEY },
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
                    { "api_key", this.mySettings.API_KEY },
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
                    { "limit", "5" },
                };

            return base.GetOrderbook(symbol, parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        { "positionId", symbol },
                        { "leverage", leverage.ToString() },
                        { "timestamp", TimeManager.UtcTimeMS().ToString() },
                        { "api_key", this.mySettings.API_KEY },
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
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "symbol", symbol },
                    { "vol", qty.ToString() },
                    { "price", price.ToString() },
                    { "openType", "2" },
                    { "type", orderType.Equals(ORDER_TYPE.limit) ? "1" : "5" },
                    { "timestamp", TimeManager.UtcTimeMS().ToString() },
                    { "api_key", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                };

            if (orderSide.Equals(ORDER_SIDE.buy))
            {
                if (orderDirection.Equals(ORDER_DIRECTION.OPEN))
                {
                    parameters.Add("side", "1");
                }
                else
                {
                    parameters.Add("side", "4");
                }
            }
            else
            {
                if (orderDirection.Equals(ORDER_DIRECTION.OPEN))
                {
                    parameters.Add("side", "3");
                }
                else
                {
                    parameters.Add("side", "2");
                }
            }

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
                    { "cancelList", orderId},
                    { "timestamp", TimeManager.UtcTimeMS().ToString() },
                    { "api_key", this.mySettings.API_KEY },
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
                    { "timestamp", TimeManager.UtcTimeMS().ToString() },
                    { "api_key", this.mySettings.API_KEY },
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