namespace DataModels.Exceptions
{
    using Configuration;
    using System;

    public class ApiCallException : Exception
    {
        public ApiCallException()
        {
            this.STATE = REQUEST_STATE.INVAILD;
        }

        public ApiCallException(Exception e)
        {
            this.STATE = REQUEST_STATE.INVAILD;
            this.e = e;
        }

        public ApiCallException(REQUEST_STATE state, Exception e)
        {
            this.STATE = state;
            this.e = e;
        }

        public Exception ActualException
        {
            get
            {
                return this.e;
            }
        }

        public REQUEST_STATE STATE { get; set; }

        private Exception e;
    }
}