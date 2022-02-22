namespace Markets.Controls.RequestControls
{
    using Common;
    using Configuration;
    using DataModels;
    using Markets.Converters;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class HuobiRequestControl : RequestControlBase
    {
        public HuobiRequestControl(IRequestFactory factory)
            : base(factory)
        {
        }

        public override AutoResetEvent GetBalance(int tId)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "AccessKeyId", this.mySettings.API_KEY },
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
                    { "AccessKeyId", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                };

            return base.GetPosition(parameters, tId);
        }

        public override AutoResetEvent GetOrderbook(string symbol, int tId)
        {
            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "contract_code", symbol },
                    { "type", "step5" },
                };

            return base.GetOrderbook(symbol, parameters, tId);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            Dictionary<string, string> parameters =
                    new Dictionary<string, string>()
                    {
                        { "lever_rate", leverage.ToString() },
                        { "contract_code", symbol },
                        { "AccessKeyId", this.mySettings.API_KEY },
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
                CoinSymbolConverter.ConvertSymbolToCoinName(COIN_MARKET.HUOBI, symbol));

            long size = (long)((qty) / (this.mySettings.GetMinTradeValue(coinType)));

            string tradeCode = string.Empty;
            string posCode = string.Empty;

            // Attenstion
            // Huobi order type is not General intuition.
            if (orderSide.Equals(ORDER_SIDE.buy) && orderDirection.Equals(ORDER_DIRECTION.OPEN))
            {
                // Open Long
                tradeCode = "buy";
                posCode = "open";
            }
            else if (orderSide.Equals(ORDER_SIDE.buy) && orderDirection.Equals(ORDER_DIRECTION.CLOSE))
            {
                // Close Long
                tradeCode = "sell";
                posCode = "close";
            }
            else if (orderSide.Equals(ORDER_SIDE.sell) && orderDirection.Equals(ORDER_DIRECTION.OPEN))
            {
                // Open Short
                tradeCode = "sell";
                posCode = "open";
            }
            else
            {
                // orderSide.Equals(ORDER_SIDE.sell)
                // orderDirection.Equals(ORDER_DIRECTION.CLOSE))
                // Close Short
                tradeCode = "buy";
                posCode = "close";
            }

            Dictionary<string, string> parameters =
                new Dictionary<string, string>()
                {
                    { "contract_code", symbol },
                    { "volume", size.ToString() },
                    { "price", price.ToString() },
                    { "direction", tradeCode },
                    { "offset", posCode },
                    { "lever_rate", "20" },
                    { "order_price_type", orderType.Equals(ORDER_TYPE.limit) ? "limit" : "market" },
                    { "AccessKeyId", this.mySettings.API_KEY },
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
                    { "contract_code", symbol},
                    { "order_id", orderId},
                    { "AccessKeyId", this.mySettings.API_KEY },
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
                    { "contract_code", symbol},
                    { "order_id", orderId},
                    { "AccessKeyId", this.mySettings.API_KEY },
                    { "SecretKey", this.mySettings.SECRET_KEY },
                };

            return base.GetOrderInfo(symbol, orderId, parameters, tId);
        }

        public override void CalcTotalPosition(IDictionary<COIN_TYPE, Position> positions)
        {
            foreach (Position position in positions.Values)
            {
                position.Total = 0;

                if (position.GetQuantity(POSITION_SIDE.LONG) > position.GetQuantity(POSITION_SIDE.SHORT))
                {
                    position.Total += position.GetPrice(POSITION_SIDE.LONG) * position.GetQuantity(POSITION_SIDE.LONG);
                }
                else
                {
                    position.Total += position.GetPrice(POSITION_SIDE.SHORT) * position.GetQuantity(POSITION_SIDE.SHORT);
                }
            }
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