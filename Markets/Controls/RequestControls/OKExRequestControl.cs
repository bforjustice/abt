namespace Markets.Controls.RequestControls
{
    using Common;
    using Configuration;
    using Markets.Converters;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class OKExRequestControl : RequestControlBase
    {
        public OKExRequestControl(IRequestFactory factory)
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
                    { "instId", symbol },
                    { "sz", "5" },
                };

            return base.GetOrderbook(symbol, parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        { "instId", symbol },
                        { "lever", leverage.ToString() },
                        { "mgnMode", "cross" },
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
                CoinSymbolConverter.ConvertSymbolToCoinName(COIN_MARKET.OKEX, symbol));

            long size = (long)((qty) / (this.mySettings.GetOrderUnit(coinType)));
            string side = string.Empty;

            if (orderDirection.Equals(ORDER_DIRECTION.OPEN))
            {
                side = orderSide.Equals(ORDER_SIDE.buy) ? "buy" : "sell";
            }
            else
            {
                side = orderSide.Equals(ORDER_SIDE.buy) ? "sell" : "buy";
            }

            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "instId", symbol },
                    { "tdMode", "cross" },
                    { "side", side },
                    { "posSide", orderSide.Equals(ORDER_SIDE.buy) ? "long" : "short" },
                    { "ordType", orderType.Equals(ORDER_TYPE.limit) ? "limit" : "market" },
                    { "sz", size.ToString() },
                    { "reduceOnly", orderDirection.Equals(ORDER_DIRECTION.CLOSE) ? "true" : "false" },
                    { "px", orderType.Equals(ORDER_TYPE.limit) ? price.ToString() : string.Empty },
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
                    { "instId", symbol},
                    { "ordId", orderId},
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
                    { "instId", symbol},
                    { "ordId", orderId},
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
                    { "instType", "SWAP" },
                };

            return base.GetTickers(parameters, tId);
        }
    }
}