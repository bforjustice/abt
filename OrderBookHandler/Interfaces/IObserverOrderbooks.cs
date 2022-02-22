namespace OrderBookHandler.Interfaces
{
    using DataModels;

    public interface IObserverOrderbooks
    {
        void Publish(OrderBooks orderbooks);
    }
}