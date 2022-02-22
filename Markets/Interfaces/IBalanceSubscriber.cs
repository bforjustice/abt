namespace Markets.Interfaces
{
    using DataModels;

    public interface IBalanceSubscriber
    {
        void PublishBalance(Balance balnace);
    }
}