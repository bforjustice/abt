namespace DataUpdater.Helper
{
    using Configuration;
    using System;

    public static class ConvertSymbolToCoinStr
    {
        public static string Convert(COIN_MARKET market, string symbol)
        {
            switch (market)
            {
                case COIN_MARKET.BINANCE:
                    return symbol.Replace("USDT", string.Empty);

                case COIN_MARKET.BYBIT:
                    return symbol.Replace("USDT", string.Empty);

                case COIN_MARKET.HUOBI:
                    return symbol.Replace("-USDT", string.Empty).ToUpper();

                case COIN_MARKET.BITGET:
                    return symbol.Replace("cmt_", string.Empty).Replace("usdt", string.Empty);

                case COIN_MARKET.FTX:
                    return symbol.Replace("-PERP", string.Empty);

                case COIN_MARKET.BITZ:
                    return ConvertContractIdByBitZ(symbol);

                case COIN_MARKET.OKEX:
                    return symbol.Replace("-USDT", string.Empty).Replace("-SWAP", string.Empty);

                case COIN_MARKET.MXC:
                    return symbol.Replace("_USDT", string.Empty);

                case COIN_MARKET.GATEIO:
                    return symbol.Replace("_USDT", string.Empty);

                case COIN_MARKET.ZBG:
                    return symbol.Replace("_USDT", string.Empty);

                default:
                    throw new NotImplementedException("Not Support Market");
            }
        }

        private static string ConvertContractIdByBitZ(string symbol)
        {
            switch (symbol)
            {
                case "101":
                    return "BTC";

                case "102":
                    return "ETH";

                case "105":
                    return "LTC";

                case "103":
                    return "BCH";

                case "121":
                    return "LINK";

                default:
                    return string.Empty;
            }
        }
    }
}