namespace Communication.Interfaces
{
    using Configuration;
    using MessageBuilders.Interfaces;
    using System.Threading;

    public interface IRequest
    {
        int Tid { get; }

        ICommunicator Communicator { get; set; }

        DATA_SOURCE DataSource { get; set; }

        void SetRequest(IGeneralRestRequest request);

        IGeneralRestRequest GetRequest();

        string GetResponse();

        AutoResetEvent Dispatch();

        string Identifier();

        REQUEST_TYPE RequestType { get; }
    }
}