namespace DataUpdater
{
    using Configuration;
    using Database.Interfaces;
    using DataModels;
    using DataUpdater.Helper;
    using OrderBookHandler.Interfaces;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class OrderbookUpdater : IObserverOrderbooks
    {
        private IDataBaseStoreController myDBCtrl;

        private AutoResetEvent stopEvent;

        private ConcurrentQueue<OrderBooks> myJobQueue;

        public OrderbookUpdater(IDataBaseStoreController dbCtrl)
        {
            this.myDBCtrl = dbCtrl;
            this.stopEvent = new AutoResetEvent(false);
            this.myJobQueue = new ConcurrentQueue<OrderBooks>();

            Task.Run(() => StoreRealDatas());
        }

        ~OrderbookUpdater()
        {
            this.stopEvent.Set();
        }

        public void Publish(OrderBooks orderbooks)
        {
            this.myJobQueue.Enqueue(orderbooks);
        }

        public void StoreRealDatas()
        {
            while (!this.stopEvent.WaitOne(1))
            {
                if (!this.myJobQueue.IsEmpty)
                {
                    OrderBooks orderBooks;
                    if (this.myJobQueue.TryDequeue(out orderBooks))
                    {
                        string symbol =
                            ConvertSymbolToCoinStr.Convert(
                                orderBooks.BuyOrderbooks.First().Market,
                                orderBooks.BuyOrderbooks.First().Symbol);

                        OrderBook minAskOrderbook = this.FindMinimumSellOrderBook(orderBooks.SellOrderbooks);
                        OrderBook maxBidOrderbook = this.FindMaximumBuyOrderBook(orderBooks.BuyOrderbooks);

                        this.myDBCtrl.StoreRealTimeData(symbol,
                            minAskOrderbook.Market.ToString(),
                            maxBidOrderbook.Market.ToString(),
                            minAskOrderbook.DepthPrice(ORDERBOOK_SIDE.ASK)[0],
                            maxBidOrderbook.DepthPrice(ORDERBOOK_SIDE.BID)[0],
                            minAskOrderbook.DepthQuantity(ORDERBOOK_SIDE.ASK)[0],
                            maxBidOrderbook.DepthQuantity(ORDERBOOK_SIDE.BID)[0]
                            );
                    }
                }
            }
        }

        private OrderBook FindMinimumSellOrderBook(IList<OrderBook> orderBooks)
        {
            double minValue = double.MaxValue;
            OrderBook maxOrderbook = null;

            foreach (OrderBook orderbook in orderBooks)
            {
                if (orderbook.DepthPrice(ORDERBOOK_SIDE.ASK)[0] < minValue)
                {
                    maxOrderbook = orderbook;
                    minValue = orderbook.DepthPrice(ORDERBOOK_SIDE.ASK)[0];
                }
            }

            return maxOrderbook;
        }

        private OrderBook FindMaximumBuyOrderBook(IList<OrderBook> orderBooks)
        {
            double maxValue = double.MinValue;
            OrderBook minOrderbook = null;

            foreach (OrderBook orderbook in orderBooks)
            {
                if (orderbook.DepthPrice(ORDERBOOK_SIDE.BID)[0] > maxValue)
                {
                    minOrderbook = orderbook;
                    maxValue = orderbook.DepthPrice(ORDERBOOK_SIDE.BID)[0];
                }
            }

            return minOrderbook;
        }
    }
}