namespace DataModels
{
    using Configuration;
    using DataModels.Exceptions;
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;

    public class APIResult
    {
        public static APIResult Empty()
        {
            APIResult result = new APIResult();
            result.STATE = REQUEST_STATE.EMPTY;
            return result;
        }

        public static APIResult Create(DATA_SOURCE source)
        {
            APIResult result = new APIResult();
            result.SOURCE = source;
            result.STATE = REQUEST_STATE.NORMAL;
            return result;
        }

        public static APIResult Create(DATA_SOURCE source, ApiCallException e)
        {
            APIResult result = new APIResult();
            result.SOURCE = source;
            result.STATE = e.STATE;
            result.Exception = e;
            result.UpdateDone();
            return result;
        }

        public static APIResult Create(DATA_SOURCE source, Exception e)
        {
            APIResult result = new APIResult();
            result.SOURCE = source;
            result.STATE = REQUEST_STATE.UNKNOWN;
            result.Exception = e;
            result.UpdateDone();
            return result;
        }

        public APIResult()
        {
            this.stopWatch = new Stopwatch();
            this.stopWatch.Start();
        }

        public override string ToString()
        {
            string msg = string.Empty;

            StringBuilder builder = new StringBuilder();

            builder.Append("\n========================================================================\n");
            builder.Append("API Result : \n");
            builder.Append($"Result : {this.Result.ToString()} \n");
            builder.Append($"Method State : {this.Method.ToString()} \n");
            builder.Append($"Idetifier : {this.Identifier} \n");
            builder.Append("\n========================================================================\n");

            return builder.ToString();
        }

        public Exception Exception;

        public REQUEST_TYPE Method { get; set; }

        public string Result { get; set; }

        public string Identifier { get; set; }

        public int Tid { get; set; }

        public AutoResetEvent DoneEvent { get; set; }

        public double ElspTime { get; set; }

        private Stopwatch stopWatch;

        public REQUEST_STATE STATE { get; private set; }

        public DATA_SOURCE SOURCE { get; private set; }

        public void UpdateDone()
        {
            this.stopWatch.Stop();
            this.ElspTime = stopWatch.ElapsedMilliseconds;
        }
    }
}