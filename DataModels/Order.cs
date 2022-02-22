namespace DataModels
{
    using Configuration;
    using System.Text;

    public class Order : ItemBase
    {
        public Order Clone()
        {
            return new Order()
            {
                Quantity = this.Quantity,
                TotalMargin = this.TotalMargin,
                OrderUnit = this.OrderUnit,
                MinTradeValue = this.MinTradeValue,
                PriceDiffThreshold = this.PriceDiffThreshold,
                MarginThreshold = this.MarginThreshold,
                MarketBound = this.MarketBound,
                Symbol = this.Symbol,
                OrderbookId = this.OrderbookId,
                OrderPrice = this.OrderPrice,
                Side = this.Side,
                Type = this.Type,
                Market = this.Market,
            };
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"----------------------ORDER INFO---------------------------------\n");
            builder.Append($"Market : {this.Market.ToString()}\n");
            builder.Append($"Quantity : {Quantity.ToString()}\n");
            builder.Append($"TotalMargin : {TotalMargin.ToString()}\n");
            builder.Append($"OrderUnit : {OrderUnit.ToString()}\n");
            builder.Append($"MinTradeValue : {MinTradeValue.ToString()}\n");
            builder.Append($"PriceDiffThreshold : {PriceDiffThreshold.ToString()}\n");
            builder.Append($"MarginThreshold : {MarginThreshold.ToString()}\n");
            builder.Append($"MarketBound : {MarketBound.ToString()}\n");
            builder.Append($"Symbol : {Symbol.ToString()}\n");
            builder.Append($"OrderbookId : {OrderbookId.ToString()}\n");
            builder.Append($"OrderPrice : {OrderPrice.ToString()}\n");
            builder.Append($"ORDER_SIDE : {Side.ToString()}\n");
            builder.Append($"ORDER_TYPE : {Type.ToString()}\n");
            builder.Append($"ORDER_DIRECTION : {Direction.ToString()}\n");
            builder.Append($"----------------------ORDER INFO-END-----------------------------");

            return builder.ToString();
        }

        // Temporary
        public double Quantity { get; set; }

        // Temporary
        public double TotalMargin { get; set; }

        // Temporary
        public double OrderUnit { get; set; }

        // Temporary
        public double MinTradeValue { get; set; }

        // Temporary - Common
        public double PriceDiffThreshold { get; set; }

        // Temporary - Common
        public double MarginThreshold { get; set; }

        // Temporary
        public int MarketBound { get; set; }

        // Temporary
        public string Symbol { get; set; }

        public int OrderbookId { get; set; }

        public double OrderPrice { get; set; } = 0;

        public ORDER_SIDE Side { get; set; }

        public ORDER_TYPE Type { get; set; }

        public ORDER_DIRECTION Direction { get; set; }
    }
}