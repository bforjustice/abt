namespace Markets.Interfaces
{
    using DataModels;

    public interface IOrderbookSubscriber
    {
        void PublishOrderbook(OrderBook orderBook);
    }
}