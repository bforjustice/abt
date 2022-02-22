namespace MessageBuilders.Loader.Tests
{
    using MessageBuilders.Loader;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;

    [TestClass()]
    public class JSONLoaderTests
    {
        [TestMethod()]
        public void LoadFileTest()
        {
            Assert.IsTrue(File.Exists("..//..//..//ConfigurationFiles//Test.json"));
            Assert.IsNotNull(JSONLoader.LoadFile("..//..//..//ConfigurationFiles//Test.json"));
        }
    }
}