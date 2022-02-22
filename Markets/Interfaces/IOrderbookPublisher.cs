namespace Markets.Interfaces
{
    using DataModels;

    public interface IOrderbookPublisher
    {
        void Subscribe(IOrderbookSubscriber subscriber);

        void Unsubscribe(IOrderbookSubscriber subscriber);

        void Notify(OrderBook orderBook);
    }
}