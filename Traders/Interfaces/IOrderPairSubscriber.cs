namespace Traders.Interfaces
{
    using DataModels;

    public interface IOrderPairSubscriber
    {
        void PublishOrderInfo(OrderInfo info);
    }
}