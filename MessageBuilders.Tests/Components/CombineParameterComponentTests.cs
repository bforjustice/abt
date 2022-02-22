namespace MessageBuilders.Components.Tests
{
    using MessageBuilders.Components;
    using MessageBuilders.Components.GeneralComponents;
    using MessageBuilders.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class CombineParameterComponentTests
    {
        [TestMethod()]
        public void CombineParameterComponentTest()
        {
            IBlockComponent<object> uriBlock = new ValueComponent("Prefix", "/orderbook");
            IBlockComponent<object> valueBlock = new ValueComponent("BaseUrl", "baseUrl.com");
            IBlockComponent<object> delimeterBlock = new ValueComponent("Delimeter", "?");

            IBlockComponent<object> combiner = new CombineParameterComponent("ParameterCombiner");

            combiner.SetSubComponent(valueBlock);
            combiner.SetSubComponent(uriBlock);
            combiner.SetSubComponent(delimeterBlock);

            Assert.IsTrue(combiner.Result.Equals("baseUrl.com/orderbook?"));
        }
    }
}