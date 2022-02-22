using DataModels;

namespace Markets.Interfaces
{
    public interface ITickerPublisher
    {
        void Subscribe(ITickerSubscriber subscriber);

        void Unsubscribe(ITickerSubscriber subscriber);

        void Notify(Tickers tickers);
    }
}