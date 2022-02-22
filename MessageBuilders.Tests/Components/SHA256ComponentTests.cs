namespace MessageBuilders.Components.Tests
{
    using MessageBuilders.Components.GeneralComponents;
    using MessageBuilders.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class SHA256ComponentTests
    {
        [TestMethod()]
        public void SHA256ComponentTest()
        {
            IParameterComponent<object> keyBlock = new ParameterComponent("SecretKey", "String");
            IBlockComponent<object> dataBlock = new ValueComponent("Data", "Test");

            SHA256Component bytes = new SHA256Component("SHA256");
            keyBlock.SetValue("2b5eb11e18796d12d88f13dc27dbbd02c2cc51ff7059765ed9821957d82bb4d9");

            bytes.SetSubComponent(keyBlock);
            bytes.SetSubComponent(dataBlock);

            Assert.IsNotNull(bytes.Result);
        }
    }
}