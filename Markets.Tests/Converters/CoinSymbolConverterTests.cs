using Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Markets.Converters.Tests
{
    [TestClass()]
    public class CoinSymbolConverterTests
    {
        [TestMethod()]
        public void ConvertCoinSymbolTest()
        {
            Assert.IsTrue(CoinSymbolConverter.ConvertCoinSymbol(COIN_MARKET.BINANCE, COIN_TYPE.XTZ).Equals("XTZUSDT"));
            Assert.IsTrue(CoinSymbolConverter.ConvertCoinSymbol(COIN_MARKET.BYBIT, COIN_TYPE.XTZ).Equals("XTZUSDT"));
            Assert.IsTrue(CoinSymbolConverter.ConvertCoinSymbol(COIN_MARKET.BITGET, COIN_TYPE.XTZ).Equals("cmt_xtzusdt"));
            //Assert.IsTrue(CoinSymbolConverter.ConvertCoinSymbol(COIN_MARKET.OKEX, COIN_TYPE.XTZ).Equals("XTZ-USDT-SWAP"));
            Assert.IsTrue(CoinSymbolConverter.ConvertCoinSymbol(COIN_MARKET.HUOBI, COIN_TYPE.XTZ).Equals("XTZ-USDT"));
        }
    }
}