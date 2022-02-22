namespace Communication.Interfaces
{
    using Configuration;
    using System.Threading;

    public interface ICommunicator : IJobPublisher
    {
        AutoResetEvent Reqeust(IRequest request);

        DATA_SOURCE COMMUNICATOR_TYPE { get; }
    }
}