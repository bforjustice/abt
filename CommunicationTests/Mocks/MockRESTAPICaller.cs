namespace CommunicationTests.Mocks
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using DataModels.Exceptions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;

    public class MockRESTAPICallerInvaildException : ICommunicator
    {
        public DATA_SOURCE COMMUNICATOR_TYPE => throw new NotImplementedException();

        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AutoResetEvent ConnectStream(string uri)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent DiscoonectStream()
        {
            throw new NotImplementedException();
        }

        public void Notify(APIResult result)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent Reqeust(IRequest request)
        {
            throw new ApiCallException(REQUEST_STATE.INVAILD, new InvalidOperationException());
        }

        public void Subscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }
    }

    public class MockRESTAPICallerWebException : ICommunicator
    {
        public DATA_SOURCE COMMUNICATOR_TYPE => throw new NotImplementedException();

        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AutoResetEvent ConnectStream(string uri)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent DiscoonectStream()
        {
            throw new NotImplementedException();
        }

        public void Notify(APIResult result)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent Reqeust(IRequest request)
        {
            throw new ApiCallException(REQUEST_STATE.EXPIRED, new WebException());
        }

        public void Subscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }
    }

    public class MockRESTAPICallerProtocolViolationException : ICommunicator
    {
        public DATA_SOURCE COMMUNICATOR_TYPE => throw new NotImplementedException();

        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AutoResetEvent ConnectStream(string uri)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent DiscoonectStream()
        {
            throw new NotImplementedException();
        }

        public void Notify(APIResult result)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent Reqeust(IRequest request)
        {
            throw new ApiCallException(REQUEST_STATE.CHUNK_INVAILD, new ProtocolViolationException());
        }

        public void Subscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }
    }

    public class MockRESTAPICallerException : ICommunicator
    {
        public DATA_SOURCE COMMUNICATOR_TYPE => throw new NotImplementedException();

        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AutoResetEvent ConnectStream(string uri)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent DiscoonectStream()
        {
            throw new NotImplementedException();
        }

        public void Notify(APIResult result)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent Reqeust(IRequest request)
        {
            throw new ApiCallException(REQUEST_STATE.UNKNOWN, new Exception());
        }

        public void Subscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }
    }

    public class MockPublisherAPI : ICommunicator
    {
        private ConcurrentQueue<Tuple<AutoResetEvent, MockRequest>> jobQueue;

        private IList<IJobSubscriber> communicationPeers;

        public DATA_SOURCE COMMUNICATOR_TYPE => throw new NotImplementedException();

        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MockPublisherAPI()
        {
            this.communicationPeers = new List<IJobSubscriber>();
            this.jobQueue = new ConcurrentQueue<Tuple<AutoResetEvent, MockRequest>>();

            Thread notifyThread = new Thread(new ThreadStart(this.NotifyContext));
            notifyThread.Start();
        }

        public AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            this.jobQueue.Enqueue(new Tuple<AutoResetEvent, MockRequest>(resetEvent, request as MockRequest));
            return resetEvent;
        }

        private void NotifyContext()
        {
            while (true)
            {
                if (!this.jobQueue.IsEmpty)
                {
                    Tuple<AutoResetEvent, MockRequest> tuple;
                    if (this.jobQueue.TryDequeue(out tuple))
                    {
                        IList<string> wRes = (tuple.Item2.GetResponse()).Split(':');
                        AutoResetEvent doneEvent = tuple.Item1;

                        APIResult result = APIResult.Create(DATA_SOURCE.REST);

                        result.Result = wRes[0];
                        result.Method = (REQUEST_TYPE)Enum.Parse(typeof(REQUEST_TYPE), wRes[1]);
                        result.DoneEvent = doneEvent;

                        this.Notify(result);
                        doneEvent.Set();
                        break;
                    }
                }
            }
        }

        public void Notify(APIResult result)
        {
            foreach (IJobSubscriber subscriber in this.communicationPeers)
            {
                subscriber.PublishJob(result);
            }
        }

        public void Subscribe(IJobSubscriber subscriber)
        {
            this.communicationPeers.Add(subscriber);
        }

        public void Unsubscribe(IJobSubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent ConnectStream(string uri)
        {
            throw new NotImplementedException();
        }

        public AutoResetEvent DiscoonectStream()
        {
            throw new NotImplementedException();
        }
    }
}