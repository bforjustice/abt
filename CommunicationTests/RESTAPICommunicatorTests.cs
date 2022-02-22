namespace Communication.Tests
{
    using Communication.Interfaces;
    using CommunicationTests.Mocks;
    using Configuration;
    using DataModels.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;

    [TestClass()]
    public class RESTAPICommunicatorTests
    {
        [TestMethod()]
        public void callGetAPIInvaildExceptionTest()
        {
            try
            {
                ICommunicator apiCall = new MockRESTAPICallerInvaildException();
                apiCall.Reqeust(null);
            }
            catch (ApiCallException e)
            {
                Assert.IsTrue(e.ActualException is InvalidOperationException);
                Assert.IsTrue(e.STATE.Equals(REQUEST_STATE.INVAILD));
            }
        }

        [TestMethod()]
        public void callGetAPIWebExceptionTest()
        {
            try
            {
                ICommunicator apiCall = new MockRESTAPICallerWebException();
                apiCall.Reqeust(null);
            }
            catch (ApiCallException e)
            {
                Assert.IsTrue(e.ActualException is WebException);
                Assert.IsTrue(e.STATE.Equals(REQUEST_STATE.EXPIRED));
            }
        }

        [TestMethod()]
        public void callGetAPIProtocolViolationExceptionTest()
        {
            try
            {
                ICommunicator apiCall = new MockRESTAPICallerProtocolViolationException();
                apiCall.Reqeust(null);
            }
            catch (ApiCallException e)
            {
                Assert.IsTrue(e.ActualException is ProtocolViolationException);
                Assert.IsTrue(e.STATE.Equals(REQUEST_STATE.CHUNK_INVAILD));
            }
        }

        [TestMethod()]
        public void callGetAPIExceptionTest()
        {
            try
            {
                ICommunicator apiCall = new MockRESTAPICallerException();
                apiCall.Reqeust(null);
            }
            catch (ApiCallException e)
            {
                Assert.IsTrue(e.ActualException is Exception);
                Assert.IsTrue(e.STATE.Equals(REQUEST_STATE.UNKNOWN));
            }
        }
    }
}