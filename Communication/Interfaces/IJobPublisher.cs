namespace Communication.Interfaces
{
    using DataModels;

    public interface IJobPublisher
    {
        void Subscribe(IJobSubscriber subscriber);

        void Unsubscribe(IJobSubscriber subscriber);

        void Notify(APIResult result);
    }
}