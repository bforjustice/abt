namespace DataUpdater.Interfaces
{
    public interface IObservableTrade
    {
        void Subscribe(IObserverTrade subscriber);

        void Unsubscribe(IObserverTrade subscriber);
    }
}