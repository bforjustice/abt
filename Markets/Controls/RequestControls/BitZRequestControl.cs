namespace Markets.Controls.RequestControls
{
    using Common;
    using Configuration;
    using Markets.Converters;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class BitZRequestControl : RequestControlBase
    {
        public BitZRequestControl(IRequestFactory factory)
            : base(factory)
        {
        }

        public override AutoResetEvent GetBalance(int tId)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);

            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "accessKey", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                };

            return base.GetBalance(parameters, tId);
        }

        public override AutoResetEvent GetPosition(int tId)
        {
            return new AutoResetEvent(true);
        }

        public override AutoResetEvent GetOrderbook(string symbol, int tId)
        {
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "contractId", symbol },
                    { "depth", "5" },
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
            COIN_TYPE coinType = (COIN_TYPE)Enum.Parse(typeof(COIN_TYPE),
                CoinSymbolConverter.ConvertSymbolToCoinName(COIN_MARKET.BITZ, symbol));

            long size = (long)((qty) / (this.mySettings.GetMinTradeValue(coinType)));

            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "contractId", symbol},
                    { "price", price.ToString() },
                    { "amount", size.ToString() },
                    { "direction", orderSide.Equals(ORDER_SIDE.buy) ? "1" : "-1" },
                    { "isCross", "1" },
                    { "leverage", "10" },
                    { "type", orderType.Equals(ORDER_TYPE.limit) ? "limit" : "market" },
                    { "accessKey", this.mySettings.API_KEY },
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
                    { "entrustSheetId", orderId},
                    { "accessKey", this.mySettings.API_KEY },
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
                    { "entrustSheetId", orderId},
                    { "accessKey", this.mySettings.API_KEY },
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