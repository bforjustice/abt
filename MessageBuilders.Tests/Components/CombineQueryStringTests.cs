namespace MessageBuilders.Components.Tests
{
    using MessageBuilders.Components;
    using MessageBuilders.Components.GeneralComponents;
    using MessageBuilders.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class CombineQueryStringTests
    {
        [TestMethod()]
        public void CombineQueryStringTest()
        {
            ParameterComponent apiKeyBlock = new ParameterComponent("apiKey", "String");
            ParameterComponent nounceBlock = new ParameterComponent("timestamp", "String");
            ParameterComponent coinType = new ParameterComponent("symbol", "String");

            IBlockComponent<object> baseBlock = new ValueComponent("BaseUrl", "baseUrl.com");
            IBlockComponent<object> prefixBlock = new ValueComponent("Prefix", "/orderbook");
            IBlockComponent<object> delimeterBlock = new ValueComponent("Delimeter", "?");

            IBlockComponent<object> querycombiner = new CombineQueryString("QueryCombiner");
            IBlockComponent<object> requestPathCombiner = new CombineRequestPathComponent("RequestPathCombiner");
            IBlockComponent<object> fullPathCombiner = new CombineFullPathComponent("FullPathCombiner");

            apiKeyBlock.SetValue("dbefbc809e3e83c283a984c3a1459732ea7db1360ca80c5c2c8867408d28cc83");
            coinType.SetValue("XTZ");
            nounceBlock.SetValue("124823582");

            requestPathCombiner.SetSubComponent(baseBlock);
            requestPathCombiner.SetSubComponent(prefixBlock);

            querycombiner.SetSubComponent(apiKeyBlock);
            querycombiner.SetSubComponent(nounceBlock);
            querycombiner.SetSubComponent(coinType);

            fullPathCombiner.SetSubComponent(requestPathCombiner);
            fullPathCombiner.SetSubComponent(delimeterBlock);
            fullPathCombiner.SetSubComponent(querycombiner);

            Assert.IsTrue(Convert.ToString(fullPathCombiner.Result).Equals("baseUrl.com/orderbook?apiKey=dbefbc809e3e83c283a984c3a1459732ea7db1360ca80c5c2c8867408d28cc83&timestamp=124823582&symbol=XTZ"));
        }
    }
}