namespace Workers.Tests.Mocks
{
    using DataModels;
    using Markets.Interfaces;

    public class MockOrderBookControl : IOrderbookSubscriber
    {
        private OrderBook myResponse = null;

        public OrderBook Result => this.myResponse;

        public void PublishOrderbook(OrderBook orderBook)
        {
            this.myResponse = orderBook;
        }
    }
}