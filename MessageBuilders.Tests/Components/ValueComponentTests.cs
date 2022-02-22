namespace MessageBuilders.Components.Tests
{
    using MessageBuilders.Components.GeneralComponents;
    using MessageBuilders.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ValueComponentTests
    {
        [TestMethod()]
        public void ValueComponentTest()
        {
            IBlockComponent<object> block = new ValueComponent("BaseUrl", "Test");

            Assert.IsTrue(block.Result.Equals("Test"));
        }
    }
}