namespace Communication
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using DataModels.Exceptions;
    using LogTrace.Interfaces;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    public class RESTAPICommunicator : CommunicatorBase, ICommunicator, IJobPublisher
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CommLogger");

        private IList<IJobSubscriber> communicationPeers;

        private ConcurrentQueue<Tuple<AutoResetEvent, IRequest>> jobQueue;

        public RESTAPICommunicator()
        {
            this.communicationPeers = new List<IJobSubscriber>();
            this.jobQueue = new ConcurrentQueue<Tuple<AutoResetEvent, IRequest>>();

            Thread notifyThread = new Thread(new ThreadStart(this.NotifyContext));
            notifyThread.Start();
        }

        public DATA_SOURCE COMMUNICATOR_TYPE => DATA_SOURCE.REST;

        public AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            this.jobQueue.Enqueue(new Tuple<AutoResetEvent, IRequest>(resetEvent, request));
            return resetEvent;
        }

        private void NotifyContext()
        {
            while (true)
            {
                if (!this.jobQueue.IsEmpty)
                {
                    Tuple<AutoResetEvent, IRequest> tuple;
                    if (this.jobQueue.TryDequeue(out tuple))
                    {
                        HttpWebResponse wRes = null;
                        AutoResetEvent doneEvent = tuple.Item1;
                        try
                        {
                            APIResult result = APIResult.Create(tuple.Item2.DataSource);

                            this.PrintRequestInfo(tuple.Item2);

                            string res = tuple.Item2.GetResponse();

                            result.Tid = tuple.Item2.Tid;
                            result.Identifier = tuple.Item2.Identifier();
                            result.Result = res;
                            result.Method = tuple.Item2.RequestType;
                            result.DoneEvent = doneEvent;
                            result.UpdateDone();
                            this.Notify(result);

                            if (!(tuple.Item2.RequestType.Equals(REQUEST_TYPE.ORDERBOOK) ||
                                tuple.Item2.RequestType.Equals(REQUEST_TYPE.GET_TICKER)))
                            {
                                myLogger.Info($"REST Result: {result.Result}");
                            }
                        }
                        catch (WebException e)
                        {
                            myLogger.Error($"Comm Error WebException : {e.StackTrace} {e.ToString()}");
                            myLogger.Error($"Comm Error {tuple.Item2.ToString()}");
                            APIResult result = APIResult.Create(DATA_SOURCE.REST, new ApiCallException(REQUEST_STATE.EXPIRED, e));
                            result.DoneEvent = doneEvent;
                            this.Notify(result);
                        }
                        catch (ProtocolViolationException e)
                        {
                            myLogger.Error($"Comm Error ProtocolViolationException : {e.StackTrace} {e.ToString()}");
                            myLogger.Error($"Comm Error {tuple.Item2.ToString()}");
                            APIResult result = APIResult.Create(DATA_SOURCE.REST, new ApiCallException(REQUEST_STATE.CHUNK_INVAILD, e));
                            result.DoneEvent = doneEvent;
                            this.Notify(result);
                        }
                        catch (InvalidOperationException e)
                        {
                            myLogger.Error($"Comm Error InvalidOperationException : {e.StackTrace} {e.ToString()}");
                            myLogger.Error($"Comm Error {tuple.Item2.ToString()}");
                            APIResult result = APIResult.Create(DATA_SOURCE.REST, new ApiCallException(REQUEST_STATE.INVAILD, e));
                            result.DoneEvent = doneEvent;
                            this.Notify(result);
                        }
                        catch (Exception e)
                        {
                            myLogger.Error($"Comm Error Exception : {e.StackTrace} {e.ToString()}");
                            APIResult result = APIResult.Create(DATA_SOURCE.REST, new ApiCallException(REQUEST_STATE.UNKNOWN, e));
                            result.DoneEvent = doneEvent;
                            this.Notify(result);
                        }
                        finally
                        {
                            if (wRes != null)
                            {
                                wRes.Close();
                            }
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }

        private void PrintRequestInfo(IRequest requst)
        {
            if (requst.RequestType.Equals(REQUEST_TYPE.ORDERBOOK) ||
                requst.RequestType.Equals(REQUEST_TYPE.GET_TICKER))
            {
                return;
            }

            StringBuilder builder = new StringBuilder();

            myLogger.Info($"Request Infomation {requst.ToString()}");

        }
    }
}