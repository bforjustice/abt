namespace CalculationEngine.Tests.Mocks
{
    using Communication.Interfaces;
    using DataModels;

    public class MockJobSubscriber : IJobSubscriber
    {
        public void PublishJob(APIResult result)
        {
            result.DoneEvent.Set();
            return;
        }
    }
}