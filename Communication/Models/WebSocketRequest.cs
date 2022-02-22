namespace Communication.Models
{
    using Configuration;
    using MessageBuilders.Interfaces;
    using System.Threading;

    public class WebSocketRequest : RequestBase
    {
        public WebSocketRequest(REQUEST_TYPE reqType, IGeneralRestRequest obj)
        {
            this.myRequestType = reqType;
            this.myRequestObject = obj;
        }

        public override IGeneralRestRequest GetRequest()
        {
            return this.myRequestObject;
        }

        public override AutoResetEvent Dispatch()
        {
            return this.Communicator.Reqeust(this);
        }

        public override string GetResponse()
        {
            throw new System.NotImplementedException();
        }
    }
}