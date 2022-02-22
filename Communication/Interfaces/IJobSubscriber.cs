namespace Communication.Interfaces
{
    using DataModels;

    public interface IJobSubscriber
    {
        void PublishJob(APIResult result);
    }
}