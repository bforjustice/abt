namespace Markets.Controls.RequestControls
{
    using Common;
    using Configuration;
    using Markets.Interfaces;
    using System.Collections.Generic;
    using System.Threading;

    public class BybitRequestControl : RequestControlBase
    {
        public BybitRequestControl(IRequestFactory factory)
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
                };

            return base.GetOrderbook(symbol, parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        { "symbol", symbol },
                        { "buy_leverage", leverage.ToString() },
                        { "sell_leverage", leverage.ToString() },
                        { "timestamp", TimeManager.UtcTimeMS().ToString() },
                        { "api_key", this.mySettings.API_KEY },
                        { "SecretKey", this.mySettings.SECRET_KEY },
                        { "recv_window", "20000" },
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
            ORDER_SIDE modifiedOrderSide = orderSide;

            if (orderDirection.Equals(ORDER_DIRECTION.CLOSE))
            {
                modifiedOrderSide = orderSide.Equals(ORDER_SIDE.buy) ? ORDER_SIDE.sell: ORDER_SIDE.buy;
            }

            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                            { "side", modifiedOrderSide.Equals(ORDER_SIDE.buy) ? "Buy" : "Sell" },
                            { "symbol", symbol },
                            { "order_type", orderType.Equals(ORDER_TYPE.limit) ? "Limit" : "Market" },
                            { "qty", qty.ToString() },
                            { "price", price.ToString() },
                            { "time_in_force", "GoodTillCancel" },
                            { "reduce_only", orderDirection.Equals(ORDER_DIRECTION.CLOSE) ? "True" : "False" },
                            { "close_on_trigger", "False" },
                            { "timestamp", TimeManager.UtcTimeMS().ToString() },
                            { "api_key", this.mySettings.API_KEY },
                            { "SecretKey", this.mySettings.SECRET_KEY },
                            { "recv_window", "20000" },
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
                    { "order_id", orderId},
                    { "timestamp", TimeManager.UtcTimeMS().ToString() },
                    { "recv_window", "20000" },
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
                    { "symbol", symbol },
                    { "order_id", TimeManager.UtcTimeMS().ToString() },
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