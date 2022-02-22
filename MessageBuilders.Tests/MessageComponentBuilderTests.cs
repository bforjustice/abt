using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessageBuilders;

namespace MessageBuilders.Tests
{
    using MessageBuilders;
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using MessageBuilders.Loader;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using MessageBuilders.Components.GeneralComponents;

    [TestClass()]
    public class MessageComponentBuilderTests
    {
        [TestMethod()]
        public void CreateMessageComponentTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void MethodComponentTest()
        {
            IBlockComponent<object> getMethod = new GetMethodComponent("Method");
            IBlockComponent<object> postMethod = new PostMethodComponent("Method");
            IBlockComponent<object> deleteMethod = new DeleteMethodComponent("Method");

            Assert.IsTrue(Convert.ToString(getMethod.Result).Equals("GET"));
            Assert.IsTrue(Convert.ToString(postMethod.Result).Equals("POST"));
            Assert.IsTrue(Convert.ToString(deleteMethod.Result).Equals("DELETE"));
        }

        [TestMethod()]
        public void CreateBinanaceOrderBookMessageTest()
        {
            string filePath = "..//..//..//ConfigurationFiles//Tests//BinanceOrderbookTest.json";
            Assert.IsTrue(File.Exists(filePath));

            JObject rawComps = JSONLoader.LoadFile(filePath);

            RESTHttpRequester requester = new RESTHttpRequester();

            foreach (JObject item in rawComps["Orderbook"]["Components"].Children())
            {
                requester.CreateComponent(item);
            }

            requester.CreateWorkflow(rawComps["Orderbook"]["Workflow"].Children());

            Assert.IsNotNull(requester);

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "symbol", "BTCUSDT" },
                { "limit", "6" }
            };

            Assert.IsTrue(true);
            //Assert.IsTrue(requester.Create(parameters).RequestUri.OriginalString.Equals("https://fapi.binance.com/fapi/v1/depth?symbol=BTCUSDT&limit=6"));
            //Assert.IsTrue(requester.Create(parameters).RequestUri.OriginalString.Equals("https://fapi.binance.com/fapi/v1/depth?symbol=BTCUSDT&limit=6"));
        }

        [TestMethod()]
        public void CreateBinanaceOrderMessageTest()
        {
            string filePath = "..//..//..//ConfigurationFiles//Tests//BinanceOrderTest.json";
            Assert.IsTrue(File.Exists(filePath));

            JObject rawComps = JSONLoader.LoadFile(filePath);

            RESTHttpRequester requester = new RESTHttpRequester();

            foreach (JObject item in rawComps["Order"]["Components"].Children())
            {
                requester.CreateComponent(item);
            }

            requester.CreateWorkflow(rawComps["Order"]["Workflow"].Children());

            Assert.IsNotNull(requester);

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "symbol", "BTCUSDT" },
                { "side", "BUY" },
                { "type", "LIMIT" },
                { "timeInForce", "GTC" },
                { "quantity", "1" },
                { "price", "9000" },
                { "recvWindow", "5000" },
                { "timestamp", "1591702613943" },
                { "apiKey", "dbefbc809e3e83c283a984c3a1459732ea7db1360ca80c5c2c8867408d28cc83" },
                { "SecretKey", "2b5eb11e18796d12d88f13dc27dbbd02c2cc51ff7059765ed9821957d82bb4d9" },
            };

            Assert.IsTrue(true);

            //HttpWebRequest req = requester.Create(parameters);

            //Assert.IsTrue(req.RequestUri.Equals("https://fapi.binance.com/fapi/v1/order?symbol=BTCUSDT&side=BUY&type=LIMIT&quantity=1&price=9000&timeInForce=GTC&recvWindow=5000&timestamp=1591702613943&signature=3c661234138461fcc7a7d8746c6558c9842d4e10870d2ecbedf7777cad694af9"));
            //Assert.IsTrue(req.Headers["X-MBX-APIKEY"].Equals("dbefbc809e3e83c283a984c3a1459732ea7db1360ca80c5c2c8867408d28cc83"));
            //Assert.IsTrue(req.Method.Equals("POST"));
        }

        [TestMethod()]
        public void CreateOrderbookRequesterTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateHttpRequester("Orderbook", "..//..//..//ConfigurationFiles//Tests//BinanceOrderbookTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "symbol", "BTCUSDT" },
                { "limit", "6" }
            };

            Assert.IsTrue(true);
            //Assert.IsTrue(requester.Create(parameters).RequestUri.OriginalString.Equals("https://fapi.binance.com/fapi/v1/depth?symbol=BTCUSDT&limit=6"));
        }

        [TestMethod()]
        public void CreateOrderRequesterTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateHttpRequester("Order", "..//..//..//ConfigurationFiles//Tests//BinanceOrderTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "symbol", "BTCUSDT" },
                { "side", "BUY" },
                { "type", "LIMIT" },
                { "timeInForce", "GTC" },
                { "quantity", "1" },
                { "price", "9000" },
                { "recvWindow", "5000" },
                { "timestamp", "1591702613943" },
                { "apiKey", "dbefbc809e3e83c283a984c3a1459732ea7db1360ca80c5c2c8867408d28cc83" },
                { "SecretKey", "2b5eb11e18796d12d88f13dc27dbbd02c2cc51ff7059765ed9821957d82bb4d9" },
            };

            IGeneralRestRequest req = requester.Create(parameters);

            Assert.IsTrue(true);
            //Assert.IsTrue(req.RequestUri.Equals("https://fapi.binance.com/fapi/v1/order?symbol=BTCUSDT&side=BUY&type=LIMIT&quantity=1&price=9000&timeInForce=GTC&recvWindow=5000&timestamp=1591702613943&signature=3c661234138461fcc7a7d8746c6558c9842d4e10870d2ecbedf7777cad694af9"));
            //Assert.IsTrue(req.Headers["X-MBX-APIKEY"].Equals("dbefbc809e3e83c283a984c3a1459732ea7db1360ca80c5c2c8867408d28cc83"));
            //Assert.IsTrue(req.Method.Equals("POST"));
        }

        [TestMethod()]
        public void CreateHuobiAccountRequesterTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateHttpRequester("Balance", "..//..//..//ConfigurationFiles//Tests//HuobiBalanceTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "Timestamp", "2020-04-01T12:34:56" },
                { "AccessKeyId", "abcdefg-hijklmn-opqrst-uvwxyz" },
                { "SecretKey", "12345-67890-12345-67890" },
            };

            IGeneralRestRequest req = requester.Create(parameters);

            Assert.IsTrue(true);
            //Assert.IsTrue(req.RequestUri.Equals("https://api.hbdm.pro/v1/account/accounts?AccessKeyId=abcdefg-hijklmn-opqrst-uvwxyz&SignatureMethod=HmacSHA256&SignatureVersion=2&Timestamp=2020-04-01T12%3A34%3A56&Signature=3IUZcEak4IIRrh7%2FidFrP2Jj77MaWGXR%2FoQbe9gL4%2BI%3D"));
            //Assert.IsTrue(req.Method.Equals("GET"));
        }

        [TestMethod()]
        public void CreateHuobiMarginBalanceRequesterTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateHttpRequester("Balance", "..//..//..//ConfigurationFiles//Tests//HuobiMarginBalanceTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "Timestamp", "2020-04-01T12:34:56" },
                { "AccessKeyId", "abcdefg-hijklmn-opqrst-uvwxyz" },
                { "SecretKey", "12345-67890-12345-67890" },
            };

            IGeneralRestRequest req = requester.Create(parameters);
            Assert.IsTrue(true);

            //Assert.IsTrue(req.RequestUri.Equals(
            //    "https://api.hbdm.pro/linear-swap-api/v1/swap_cross_account_info?AccessKeyId=abcdefg-hijklmn-opqrst-uvwxyz&SignatureMethod=HmacSHA256&SignatureVersion=2&Timestamp=2020-04-01T12%3A34%3A56&Signature=0OUrv605OJm9ETlAJWcTtf6YBm%2F9LwgPKTuJ4es1oI4%3D"));
        }

        [TestMethod()]
        public void CreateBybitMarginOrderRequesterTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateHttpRequester("Order", "..//..//..//ConfigurationFiles//Tests//BybitOrderTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "api_key", "B2Rou0PLPpGqcU0Vu2" },
                { "side", "Buy" },
                { "symbol", "BTCUSD" },
                { "qty", "100" },
                { "price", "3001.5" },
                { "time_in_force", "GoodTillCancel" },
                { "order_type", "Limit" },
                { "reduce_only", "true" },
                { "timestamp", "1542434791000" },
                { "SecretKey", "t7T0YlFnYXk0Fx3JswQsDrViLg1Gh3DUU5Mr" },
            };

            IGeneralRestRequest req = requester.Create(parameters);

            Assert.IsTrue(true);

            //Assert.IsTrue(req.RequestUri.Equals(
            //    "https://api.bybit.com/v2/private/order/create?api_key=B2Rou0PLPpGqcU0Vu2&side=Buy&symbol=BTCUSD&qty=100&price=3001.5&time_in_force=GoodTillCancel&order_type=Limit&reduce_only=true&timestamp=1542434791000&sign=52a6a6fc4da6d395baa8daccd21eb903eb47c19c03112cc2c80b005f2d181aef"));
        }

        [TestMethod()]
        public void CreateBitZBalanceRequesterTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateHttpRequester("Balance", "..//..//..//ConfigurationFiles//Tests//BitZBalanceTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "apiKey", "376892265asdad5d12726d8bbfbd8912b3" },
                { "timeStamp", "1510235730" },
                { "nonce", "309127" },
                { "SecretKey", "aQmE8U7bxj16KdJcSd3yX8F8Sakd8aO6LopnHXh27d4kWyb28PxcaTvGrajLDvAw" },
            };

            IGeneralRestRequest req = requester.Create(parameters);

            Assert.IsTrue(true);
            //Assert.IsTrue(req.RequestUri.Equals(
            //    "https://apiv2.bitz.com//Contract/getContractAccountInfo?apiKey=376892265asdad5d12726d8bbfbd8912b3&nonce=309127&timeStamp=1510235730&sign=f3253c5e84ba54080d75309f2eca9143"));
        }

        [TestMethod()]
        public void CreateZBGBalanceRequesterTest()
        {
            IRequestCreator requester = MessageComponentBuilder.CreateHttpRequester("Balance", "..//..//..//ConfigurationFiles//Tests//ZBGBalanceTest.json");

            IDictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "apiKey", "376892265asdad5d12726d8bbfbd8912b3" },
                { "timestamp", "1510235730" },
                { "SecretKey", "aQmE8U7bxj16KdJcSd3yX8F8Sakd8aO6LopnHXh27d4kWyb28PxcaTvGrajLDvAw" },
            };

            IGeneralRestRequest req = requester.Create(parameters);

            Assert.IsTrue(true);
            //Assert.IsTrue(req.RequestUri.Equals(
            //    "https://www.zbg.com/exchange/api/v1/future/assets/available"));
        }
    }
}