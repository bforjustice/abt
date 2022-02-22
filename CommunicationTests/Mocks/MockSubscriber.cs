namespace CommunicationTests.Mocks
{
    using Communication.Interfaces;
    using DataModels;
    using System.Collections.Generic;

    public class MockSubscriber : IJobSubscriber
    {
        public IList<string> Results { set; get; }

        public void PublishJob(APIResult result)
        {
            Results = new List<string>() { result.Method.ToString(), result.Result };
        }
    }
}