namespace TidGenerators
{
    public static class TidGenerator
    {
        private static int myTid = 0;

        private static object lockObj = new object();

        public static int GenerateTransactionId()
        {
            lock (lockObj)
            {
                myTid++;
            }

            return myTid;
        }
    }
}