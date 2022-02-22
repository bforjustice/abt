namespace Common.Tests
{
    using Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class CommonApiTests
    {
        [TestMethod()]
        public void getDecimalLengthTest()
        {
            double testValue_1 = 100.1231;
            double testValue_2 = 12100.122131;
            Assert.IsTrue(CommonApi.getDecimalLength(testValue_1) == 4);
            Assert.IsTrue(CommonApi.getDecimalLength(testValue_2) == 6);
        }

        [TestMethod()]
        public void cutDecimalNumberTest()
        {
            double testValue_1 = 10.435;
            double testValue_2 = 10.432524;
            double testValue_3 = 10.432514;

            double testValue_Res = 10.4325;

            Assert.IsTrue((CommonApi.cutDecimalNumber(testValue_1, 4) - testValue_1).Equals(0));
            Assert.IsTrue((CommonApi.cutDecimalNumber(testValue_2, 4) - testValue_Res).Equals(0));
            Assert.IsTrue((CommonApi.cutDecimalNumber(testValue_3, 4) - testValue_Res).Equals(0));
        }

        [TestMethod()]
        public void compareDecimalLengthTest()
        {
            double minValue = 0.001;

            double qty_1 = 0.1;
            double qty_2 = 0.2;
            double qty_3 = 0.002;
            double qty_4 = 0.0002;

            Assert.IsTrue(CommonApi.compareDecimalLength(qty_1, minValue));
            Assert.IsTrue(CommonApi.compareDecimalLength(qty_2, minValue));
            Assert.IsTrue(CommonApi.compareDecimalLength(qty_3, minValue));
            Assert.IsFalse(CommonApi.compareDecimalLength(qty_4, minValue));
        }
    }
}