namespace DataModels
{
    using Configuration;
    using System;
    using System.Text;

    public class OrderInfo : ItemBase
    {
        //public static OrderInfo Invaild(Exception e)
        //{
        //    OrderInfo orderInfo = new OrderInfo();
        //    orderInfo.Exception = e;
        //    orderInfo.State = REQUEST_STATE.ERROR;
        //    return orderInfo;
        //}

        public static OrderInfo Invaild(COIN_MARKET market, Exception e)
        {
            OrderInfo orderInfo = new OrderInfo();
            orderInfo.Market = market;
            orderInfo.Exception = e;
            orderInfo.State = REQUEST_STATE.ERROR;
            return orderInfo;
        }

        public override string ToString()
        {
            string msg = string.Empty;

            StringBuilder builder = new StringBuilder();

            builder.Append("========================================================================\n");
            builder.Append("Order Info : \n");
            builder.Append($"Market : {this.Market.ToString()} \n");
            builder.Append($"Pending State : {this.PendingType.ToString()} \n");
            builder.Append($"EntryPrice : {this.EntryPrice} \n");
            builder.Append($"AvgPrice : {this.AvgPrice} \n");
            builder.Append($"RemainQty : {this.RemainQty} \n");
            builder.Append($"FilledQty : {this.FilledQty} \n");
            builder.Append($"OrderId : {this.OrderId} \n");
            builder.Append($"Identifier : {this.Identifier} \n");
            builder.Append($"OrderbookId : {this.OrderbookId} \n");
            builder.Append($"CoinType : {this.CoinType.ToString()} \n");
            builder.Append($"Side : {this.Side.ToString()} \n");
            builder.Append($"Type : {this.Type.ToString()} \n");
            builder.Append("========================================================================\n");

            return builder.ToString();
        }

        public PENDING_TYPE PendingType { get; set; } = PENDING_TYPE.FULL;

        public double EntryPrice { get; set; } = 0;

        public double AvgPrice { get; set; } = 0;

        public double RemainQty { get; set; } = 0;

        public double FilledQty { get; set; } = 0;

        public string OrderId { get; set; } = string.Empty;

        public string Identifier { get; set; } = string.Empty;

        public int OrderbookId { get; set; } = 0;

        public COIN_TYPE CoinType { get; set; }

        public ORDER_SIDE Side { get; set; }

        public REQUEST_TYPE Type { get; set; }
    }
}