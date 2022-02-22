namespace Communication.Models
{
    using Communication.Interfaces;
    using Configuration;
    using MessageBuilders.Interfaces;
    using System.Threading;

    public abstract class RequestBase : IRequest
    {
        protected REQUEST_TYPE myRequestType;

        protected DATA_SOURCE myDataSource;

        protected ICommunicator myComm;

        public abstract IGeneralRestRequest GetRequest();

        public abstract AutoResetEvent Dispatch();

        public abstract string GetResponse();

        public void SetRequest(IGeneralRestRequest request)
        {
            this.myRequestObject = request;
        }

        public string Identifier()
        {
            return this.myIdentifier;
        }


        public int Tid => this.myTid;

        protected IGeneralRestRequest myRequestObject;

        protected string myIdentifier;

        protected int myTid;

        public ICommunicator Communicator { get => this.myComm; set => this.myComm = value; }

        public DATA_SOURCE DataSource { get { return this.myDataSource; } set { this.myDataSource = value; } }

        public REQUEST_TYPE RequestType { get { return this.myRequestType; } }
    }
}