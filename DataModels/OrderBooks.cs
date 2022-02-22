namespace DataModels
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OrderBooks
    {
        private int oId = int.MaxValue;

        private string timeStamp = string.Empty;

        private IList<OrderBook> myBuyOrderbooks;

        private IList<OrderBook> mySellOrderbooks;

        public int OrderbookId => this.oId;

        public IList<OrderBook> BuyOrderbooks => this.myBuyOrderbooks;

        public IList<OrderBook> SellOrderbooks => this.mySellOrderbooks;

        private string mySymbol = string.Empty;

        public OrderBooks()
        {
            timeStamp = DateTime.UtcNow.ToString();
        }

        public OrderBooks(int oId, IList<OrderBook> buyOrderbooks, IList<OrderBook> sellOrderbooks)
            : base()
        {
            this.oId = oId;
            this.myBuyOrderbooks = buyOrderbooks;
            this.mySellOrderbooks = sellOrderbooks;
        }

        public OrderBook GetOrderbook(ORDER_SIDE side, COIN_MARKET market)
        {
            //IList<OrderBook> list = side.Equals(ORDER_SIDE.buy) ? this.myBuyOrderbooks : this.mySellOrderbooks;
            IList<OrderBook> list = side.Equals(ORDER_SIDE.buy) ? this.mySellOrderbooks : this.myBuyOrderbooks;

            return list.Where(e => e.Market.Equals(market)).First();
        }
    }
}