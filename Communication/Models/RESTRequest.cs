namespace Communication.Models
{
    using Configuration;
    using MessageBuilders.Interfaces;
    using System.Net;
    using System.Text;
    using System.Threading;

    public class RESTRequest : RequestBase
    {
        public RESTRequest(REQUEST_TYPE reqType, IGeneralRestRequest obj, string identifier, int tId)
        {
            this.myRequestType = reqType;
            this.myRequestObject = obj;
            this.myIdentifier = identifier;
            this.myTid = tId;
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
            return this.myRequestObject.GetResponse();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"\nType : {this.myRequestType.ToString()}\n");
            builder.Append($"Identifier : {this.myIdentifier}\n");
            builder.Append($"Tid : {this.myTid}\n");
            builder.Append($"Request Info : {this.myRequestObject.ToString()}\n");

            return builder.ToString();
        }
    }
}