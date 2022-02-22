namespace CalculationEngine.Tests.Mocks
{
    using DataModels;
    using Markets.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class MockObserverOrderbook : IOrderbookSubscriber
    {
        private IList<OrderBook> myOrderbooks;

        public MockObserverOrderbook()
        {
            this.myOrderbooks = new List<OrderBook>();
        }

        public void PublishOrderbook(OrderBook orderBook)
        {
            myOrderbooks.Add(orderBook);
        }

        public OrderBook GetOrderBook()
        {
            return myOrderbooks.Last();
        }

        public IList<OrderBook> GetOrderBooks()
        {
            return myOrderbooks;
        }
    }
}