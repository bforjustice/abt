namespace Markets.Interfaces
{
    using DataModels;

    public interface ITickerSubscriber
    {
        void PublishTickers(Tickers tickers);
    }
}