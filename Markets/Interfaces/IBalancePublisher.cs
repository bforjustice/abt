namespace Markets.Interfaces
{
    using DataModels;

    public interface IBalancePublisher
    {
        void Subscribe(IBalanceSubscriber subscriber);

        void Unsubscribe(IBalanceSubscriber subscriber);

        void Notify(Balance balnace);
    }
}