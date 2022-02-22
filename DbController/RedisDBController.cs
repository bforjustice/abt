namespace Database
{
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RedisDBController
    {
        private static object mySyncLock;

        private static ConnectionMultiplexer myServer = null;

        public RedisDBController(string uri)
        {
            mySyncLock = mySyncLock ?? new object();

            lock (mySyncLock)
            {
                myServer = myServer ?? ConnectionMultiplexer.Connect(uri); 
            }
        }

        public void Publish(string channel, string value)
        {

        }

        public void OrderdAdd(string key, string value, float score)
        {
            myServer.GetDatabase().SortedSetAdd(key, value, score);
        }
    }
}
