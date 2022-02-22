using Configuration;
using Markets.Interfaces;
using System.Collections.Generic;

namespace Workers
{
    public abstract class WorkerBase
    {
        protected IMarket myMarket;

        protected IDictionary<string, string> myParams;

        protected REQUEST_TYPE myType = REQUEST_TYPE.NONE;

        public WorkerBase(IMarket market)
        {
            this.myParams = new Dictionary<string, string>();
            this.myMarket = market;
        }
    }
}