namespace Workers
{
    using Configuration;
    using Markets.Interfaces;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Workers.Interfaces;

    public class RESTAPIWorker : WorkerBase, IWorker, IDisposable
    {
        private Thread jobThread;

        protected AutoResetEvent stopEvent;

        private bool isStarted = false;

        public RESTAPIWorker(IMarket market) :
            base(market)
        {
            this.stopEvent = new AutoResetEvent(false);
        }

        ~RESTAPIWorker()
        {
            this.Dispose();
        }

        public void Start()
        {
            if (!this.isStarted)
            {
                this.isStarted = true;
                this.jobThread = new Thread(this.DoWork);
                this.jobThread.Start();
            }
        }

        public void Stop()
        {
            this.isStarted = false;
            this.stopEvent.Set();
        }

        public void Dispose()
        {
            this.Stop();
        }

        public void Requset(REQUEST_TYPE request, string rawJsonParam)
        {
            this.myType = request;
            this.myParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJsonParam);
        }

        private void DoWork()
        {
            while (!this.stopEvent.WaitOne(300))
            {
                if (this.myType.Equals(REQUEST_TYPE.NONE))
                {
                    continue;
                }

                try
                {
                    switch (this.myType)
                    {
                        case REQUEST_TYPE.ORDERBOOK:
                            COIN_TYPE coinType;
                            if (Enum.TryParse<COIN_TYPE>(this.GetParam("symbol"), out coinType))
                            {
                                AutoResetEvent events = this.myMarket.GetOrderBook(coinType, false);
                                events.Set();
                            }
                            break;
                    }
                }
                catch (KeyNotFoundException e)
                {
                    continue;
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }

        private string GetParam(string key)
        {
            string result = string.Empty;
            if (this.myParams.TryGetValue(key, out result))
            {
                return result;
            }

            return result;
        }
    }
}