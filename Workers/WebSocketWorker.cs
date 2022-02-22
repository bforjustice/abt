namespace Workers
{
    using Configuration;
    using Markets.Interfaces;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using Workers.Interfaces;

    public class WebSocketWorker : WorkerBase, IWorker, IDisposable
    {
        public WebSocketWorker(IMarket market)
            : base(market)
        {
        }

        ~WebSocketWorker()
        {
            this.Dispose();
        }

        public void Start()
        {
        }

        public void Stop()
        {
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

        private string GetParam(string key)
        {
            string result = string.Empty;
            if (this.myParams.TryGetValue(key, out result))
            {
                return result;
            }

            throw new KeyNotFoundException("No Have Key");
        }
    }
}