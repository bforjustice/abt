using LogTrace.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogTrace.Tests
{
    [TestClass()]
    public class LoggerTests
    {
        [TestMethod()]
        public void GetAppenderTest()
        {
            ILogger log = LogTraceService.Instance.GetLogger("CalcLogger");

            Assert.IsNotNull(log);
        }
    }
}