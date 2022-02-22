namespace Markets.Tests.Mocks
{
    using Communication.Models;
    using MessageBuilders.Interfaces;
    using System.Threading;

    public class MockRequest : RequestBase
    {
        public override AutoResetEvent Dispatch()
        {
            throw new System.NotImplementedException();
        }

        public override IGeneralRestRequest GetRequest()
        {
            return this.myRequestObject;
        }

        public override string GetResponse()
        {
            throw new System.NotImplementedException();
        }
    }
}