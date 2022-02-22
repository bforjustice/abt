namespace DataModels
{
    using Configuration;
    using DataModels.Exceptions;
    using System;

    public class OrderBook : ItemBase
    {
        public static OrderBook Empty()
        {
            OrderBook orderBook = new OrderBook();
            orderBook.State = REQUEST_STATE.EMPTY;
            return orderBook;
        }

        public static OrderBook Create()
        {
            OrderBook orderBook = new OrderBook();
            orderBook.State = REQUEST_STATE.NORMAL;
            return orderBook;
        }

        public static OrderBook Create(ApiCallException e)
        {
            OrderBook orderBook = new OrderBook();
            orderBook.State = e.STATE;
            orderBook.Exception = e;
            return orderBook;
        }

        public static OrderBook Create(Exception e)
        {
            OrderBook orderBook = new OrderBook();
            orderBook.State = REQUEST_STATE.UNKNOWN;
            return orderBook;
        }

        public OrderBook()
        {
        }

        public COIN_TYPE CoinType { get; set; }

        public double[] Tradable_USDT { get; set; } = new double[Constants.ORDERBOOK_SIDE_COUNT];

        public double[] DepthQuantity(ORDERBOOK_SIDE side)
        {
            return side.Equals(ORDERBOOK_SIDE.BID) ? BidQuantity : AskQuantity;
        }

        public double[] DepthPrice(ORDERBOOK_SIDE side)
        {
            return side.Equals(ORDERBOOK_SIDE.BID) ? BidPrice : AskPrice;
        }

        public double[] DepthQuantity(ORDER_SIDE side)
        {
            return side.Equals(ORDER_SIDE.sell) ? BidQuantity : AskQuantity;
        }

        public double[] DepthPrice(ORDER_SIDE side)
        {
            return side.Equals(ORDER_SIDE.sell) ? BidPrice : AskPrice;
        }

        public void setDepth(ORDERBOOK_SIDE side, double price, double qty, int depth)
        {
            if (side.Equals(ORDERBOOK_SIDE.ASK))
            {
                AskQuantity[depth] = qty;
                AskPrice[depth] = price;
            }
            else if (side.Equals(ORDERBOOK_SIDE.BID))
            {
                BidQuantity[depth] = qty;
                BidPrice[depth] = price;
            }
        }

        private double[] AskQuantity { get; set; } = new double[Constants.ORDERBOOK_MAX_SIZE];

        private double[] AskPrice { get; set; } = new double[Constants.ORDERBOOK_MAX_SIZE];

        private double[] BidQuantity { get; set; } = new double[Constants.ORDERBOOK_MAX_SIZE];

        private double[] BidPrice { get; set; } = new double[Constants.ORDERBOOK_MAX_SIZE];

        public double PriceDiff
        {
            get { return (BidPrice[0] - AskPrice[0]); }
        }

        public double ElspTime { get; set; }

        // Temporary
        public string Symbol { get; set; }

        // Temporary
        public double TakerFee { get; set; }

        // Teomporary
        public int DecimalLength { get; set; }

        // Temporary
        public double OrderUnit { get; set; }

        // Temporary
        public double MarginThreshold { get; set; }

        // Temporary
        public double MinTradeValue { get; set; }

        // Temporary
        public double PriceDiffThreshold { get; set; }

        // Temporary
        public int Bound { get; set; }
    }
}