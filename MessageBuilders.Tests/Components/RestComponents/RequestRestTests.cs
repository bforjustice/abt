namespace MessageBuilders.Components.RestComponents.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MessageBuilders.Components.RestComponents;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MessageBuilders.Interfaces;
    using MessageBuilders.Components.Interfaces;
    using MessageBuilders.Components.GeneralComponents;
    using MessageBuilders.Components.RESTBuilder.OptionModels;
    using Common;

    [TestClass()]
    public class RequestRestTests
    {
        [TestMethod()]
        public void AddPathParamTest()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "settle", "usdt" },
            };

            AddPathParam addPathParam = new AddPathParam("AddPathParam");
            RequestOptions opts = new RequestOptions();

            foreach (string key in parameters.Keys)
            {
                addPathParam.SetComponent(new ValueComponent(key, parameters[key]));
            }

            addPathParam.Do(opts);

            RequestOptions res = addPathParam.Result;

            Assert.IsTrue(res.PathParameters.ContainsKey("settle") && res.PathParameters.ContainsValue("usdt"));
        }

        [TestMethod()]
        public void AddHeaderParamTest()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "Accept", "application/json" },
                { "Content-Type", "application/x-www-form-urlencoded" },
            };

            AddHeaderParam addPathParam = new AddHeaderParam("AddHeaderParam");
            RequestOptions opts = new RequestOptions();

            string currentTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            foreach (string key in parameters.Keys)
            {
                addPathParam.SetComponent(new ValueComponent(key, parameters[key]));
            }

            addPathParam.Do(opts);

            RequestOptions res = addPathParam.Result;

            Assert.IsTrue(res.HeaderParameters.ContainsKey("Accept") && res.HeaderParameters.Values.Any());
            Assert.IsTrue(res.HeaderParameters.ContainsKey("Content-Type") && res.HeaderParameters.Values.Any());
        }

        [TestMethod()]
        public void AddQueryParamTest()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "timestamp", TimeManager.UtcTimeMS().ToString() },
                { "symbol", "BTC" },
                { "leverage", "20" },
                { "apiKey", "testApiKey" },
            };

            AddQueryParam addPathParam = new AddQueryParam("AddHeaderParam");
            RequestOptions opts = new RequestOptions();

            string currentTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            foreach (string key in parameters.Keys)
            {
                addPathParam.SetComponent(new ValueComponent(key, parameters[key]));
            }

            addPathParam.Do(opts);

            RequestOptions res = addPathParam.Result;

            Assert.IsTrue(res.QueryParameters.ContainsKey("timestamp") && res.QueryParameters.Values.Any());
            Assert.IsTrue(res.QueryParameters.ContainsKey("symbol") && res.QueryParameters.Values.Any());
            Assert.IsTrue(res.QueryParameters.ContainsKey("leverage") && res.QueryParameters.Values.Any());
            Assert.IsTrue(res.QueryParameters.ContainsKey("apiKey") && res.QueryParameters.Values.Any());
        }

        [TestMethod()]
        public void GetBalanceResponseTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateRequester("Balance", "..//..//..//ConfigurationFiles//Tests//RestRequestTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "apiKey", "9f9adba01440f5bc3b66f7758fcd2a2b" },
                { "settle", "usdt" },
                { "SecretKey", "6d73ddbe09920a19c02041455050a98f104e2241a6ae8bcd0d6af131c91cc6e2" },
            };

            IGeneralRestRequest req = requester.Create(parameters);

            string res = req.GetResponse();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetLeverageResponseTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateRequester("Leverage", "..//..//..//ConfigurationFiles//Tests//RestRequestTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "apiKey", "9f9adba01440f5bc3b66f7758fcd2a2b" },
                { "settle", "usdt" },
                { "leverage", "5" },
                { "contract", "BTC_USDT" },
                { "SecretKey", "6d73ddbe09920a19c02041455050a98f104e2241a6ae8bcd0d6af131c91cc6e2" },
            };

            IGeneralRestRequest req = requester.Create(parameters);

            string res = req.GetResponse();

            Assert.IsTrue(true);
        }
    }
}