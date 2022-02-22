namespace OrderBookHandler.Interfaces
{
    public interface IObservableOrderbooks
    {
        void Subscribe(IObserverOrderbooks subscriber);

        void Unsubscribe(IObserverOrderbooks subscriber);
    }
}