namespace Markets.Interfaces
{
    using DataModels;

    public interface IOrderInfoPublisher
    {
        void Subscribe(IOrderInfoSubscriber subscriber);

        void Unsubscribe(IOrderInfoSubscriber subscriber);

        void Notify(OrderInfo orderInfo);
    }
}