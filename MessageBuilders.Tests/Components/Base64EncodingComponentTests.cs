namespace MessageBuilders.Components.Tests
{
    using MessageBuilders.Components.GeneralComponents;
    using MessageBuilders.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class Base64EncodingComponentTests
    {
        [TestMethod()]
        public void Base64EncodingComponentTest()
        {
            IParameterComponent<object> keyBlock = new ParameterComponent("SecretKey", "String");
            IBlockComponent<object> dataBlock = new ValueComponent("Data", "Test");

            SHA256Component bytes = new SHA256Component("Hash");
            Base64EncodingComponent encoded = new Base64EncodingComponent("Base64Encoded");

            keyBlock.SetValue("2b5eb11e18796d12d88f13dc27dbbd02c2cc51ff7059765ed9821957d82bb4d9");

            bytes.SetSubComponent(keyBlock);
            bytes.SetSubComponent(dataBlock);

            encoded.SetSubComponent(bytes);

            Assert.IsTrue(Convert.ToString(encoded.Result).Equals("juI8uEpewwm99QmWlGJ12IYJJDeZm7Q23VVph2QuAA0="));
        }
    }
}