namespace OrderBookHandler.Tests.Mocks
{
    using DataModels;
    using Markets.Interfaces;

    public class MockSubscriber : IOrderbookSubscriber
    {
        public OrderBook OrderBook { get; set; }

        public void PublishOrderbook(OrderBook orderbook)
        {
            this.OrderBook = orderbook;
        }
    }
}