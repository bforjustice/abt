namespace CalculationEngine.Tests.Mocks
{
    using Communication.Interfaces;
    using Communication.Models;
    using Configuration;
    using MessageBuilders.Interfaces;
    using System.Threading;

    public class MockRequest : RequestBase, IRequest
    {
        public MockRequest(REQUEST_TYPE reqType, IGeneralRestRequest obj, string identifier, int tId)
        {
            this.myRequestType = reqType;
            this.myRequestObject = obj;
            this.myIdentifier = identifier;
            this.myTid = tId;
        }

        public override AutoResetEvent Dispatch()
        {
            return this.Communicator.Reqeust(this);
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