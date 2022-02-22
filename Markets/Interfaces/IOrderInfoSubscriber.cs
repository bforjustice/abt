namespace Markets.Interfaces
{
    using DataModels;

    public interface IOrderInfoSubscriber
    {
        void PublishOrderInfo(OrderInfo orderInfo);
    }
}