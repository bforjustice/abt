namespace Markets.Controls.RequestControls
{
    using Configuration;
    using Markets.Converters;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class GateIORequestControl : RequestControlBase
    {
        public GateIORequestControl(IRequestFactory factory)
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
                    { "settle", "usdt" },
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
                    { "settle", "usdt" },
                };

            return base.GetPosition(parameters, tId);
        }

        public override AutoResetEvent GetOrderbook(string symbol, int tId)
        {
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "settle", "usdt" },
                    { "contract", symbol },
                    { "limit", "10" },
                };

            return base.GetOrderbook(symbol, parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        //{ "leverage", leverage.ToString() },
                        { "leverage", "5" },
                        { "contract", symbol },
                        { "apiKey", this.mySettings.API_KEY },
                        { "SecretKey", this.mySettings.SECRET_KEY },
                        { "settle", "usdt" },
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
                CoinSymbolConverter.ConvertSymbolToCoinName(COIN_MARKET.GATEIO, symbol));

            long size = (long)((qty) / (this.mySettings.GetOrderUnit(coinType)));

            if (orderDirection.Equals(ORDER_DIRECTION.CLOSE) &&
                orderSide.Equals(ORDER_SIDE.buy))
            {
                size = Math.Abs(size) * (-1);
            }
            else if (orderDirection.Equals(ORDER_DIRECTION.CLOSE) &&
                orderSide.Equals(ORDER_SIDE.sell))
            {
                size = Math.Abs(size);
            }
            else if (orderDirection.Equals(ORDER_DIRECTION.OPEN) &&
                orderSide.Equals(ORDER_SIDE.buy))
            {
                size = Math.Abs(size);
            }
            else
            {
            //(orderDirection.Equals(ORDER_DIRECTION.OPEN) &&
            //    orderSide.Equals(ORDER_SIDE.sell))

                size = Math.Abs(size) * -1;
            }

            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "contract", symbol},
                    { "size", size.ToString() },
                    { "price", orderType.Equals(ORDER_TYPE.limit) ? price.ToString() : "0" },
                    { "tif", "gtc" },
                    { "iceberg", "0" },
                    { "reduce_only", orderDirection.Equals(ORDER_DIRECTION.CLOSE) ? "true" : "false" },
                    { "apiKey", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                    { "settle", "usdt" },
                };

            return base.PlaceOrder(symbol, price, size, orderSide, orderDirection, orderType, parameters, tId);
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
                    { "settle", "usdt" },
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
                    { "settle", "usdt" },
                    { "status", "finished" },
                    { "contract", "ETH_USDT" },
                };

            return base.GetOrderInfo(symbol, orderId, parameters, tId);
        }

        public override AutoResetEvent GetTickers(int tId)
        {
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "settle", "usdt" },
                };

            return base.GetTickers(parameters, tId);
        }
    }
}