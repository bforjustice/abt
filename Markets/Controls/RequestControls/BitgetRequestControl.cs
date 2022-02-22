namespace Markets.Controls.RequestControls
{
    using Common;
    using Configuration;
    using Markets.Converters;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class BitgetRequestControl : RequestControlBase
    {
        public BitgetRequestControl(IRequestFactory factory)
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
                    { "limit", "6" },
                };

            return base.GetOrderbook(symbol, parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        { "symbol", symbol },
                        { "leverage", leverage.ToString() },
                        { "side", "1" },
                        { "holdSide", "1" },
                        { "timestamp", TimeManager.UtcTimeMS().ToString() },
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
            COIN_TYPE coinType = (COIN_TYPE)Enum.Parse(typeof(COIN_TYPE),
                CoinSymbolConverter.ConvertSymbolToCoinName(COIN_MARKET.BITGET, symbol));

            int size = ((int)((qty * 1000) / (this.mySettings.GetMinTradeValue(coinType) * 1000)));

            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        { "symbol", symbol },
                        { "client_oid", TimeManager.UtcTimeMS().ToString() },
                        { "size", size.ToString() },
                        { "order_type", "0" },
                        { "timestamp", TimeManager.UtcTimeMS().ToString() },
                        { "apiKey", this.mySettings.API_KEY },
                        { "SecretKey", this.mySettings.SECRET_KEY },
                    };

            if (orderSide.Equals(ORDER_SIDE.buy))
            {
                if (orderDirection.Equals(ORDER_DIRECTION.OPEN))
                {
                    parameters.Add("type", "1");
                }
                else
                {
                    parameters.Add("type", "4");
                }
            }
            else
            {
                if (orderDirection.Equals(ORDER_DIRECTION.OPEN))
                {
                    parameters.Add("type", "2");
                }
                else
                {
                    parameters.Add("type", "3");
                }
            }

            if (orderType.Equals(ORDER_TYPE.limit))
            {
                parameters.Add("match_price", "0");
                parameters.Add("price", price.ToString());
            }
            else
            {
                parameters.Add("match_price", "1");
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
                    { "symbol", symbol },
                    { "orderId", orderId },
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
                    { "symbol", symbol },
                    { "orderId", orderId },
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