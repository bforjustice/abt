namespace Markets.Converters
{
    using Configuration;
    using System;

    public static class CoinSymbolConverter
    {
        public static string ConvertCoinSymbol(COIN_MARKET market, COIN_TYPE coinType)
        {
            switch (market)
            {
                case COIN_MARKET.BINANCE:
                    return string.Concat(coinType.ToString(), "USDT");

                case COIN_MARKET.BYBIT:
                    return string.Concat(coinType.ToString(), "USDT");

                case COIN_MARKET.HUOBI:
                    return string.Concat(coinType.ToString(), "-", "USDT").ToUpper();

                case COIN_MARKET.BITGET:
                    return string.Concat("cmt_", coinType.ToString().ToLower(), "usdt");

                case COIN_MARKET.FTX:
                    return string.Concat(coinType.ToString().ToUpper(), "-PERP");

                case COIN_MARKET.BITZ:
                    return ConvertContractIdByBitZ(coinType);

                case COIN_MARKET.OKEX:
                    return string.Concat(coinType.ToString(), "-", "USDT", "-", "SWAP");

                case COIN_MARKET.MXC:
                    return string.Concat(coinType.ToString(), "_", "USDT");

                case COIN_MARKET.GATEIO:
                    return string.Concat(coinType.ToString(), "_", "USDT");

                case COIN_MARKET.ZBG:
                    return string.Concat(coinType.ToString(), "_", "USDT");

                default:
                    throw new NotImplementedException("Not Support Market");
            }
        }

        public static string ConvertSymbolToCoinName(COIN_MARKET market, string symbol)
        {
            switch (market)
            {
                case COIN_MARKET.BINANCE:
                    return symbol.Substring(symbol.Length - "USDT".Length).Equals("USDT") ? symbol.Replace("USDT", string.Empty) : string.Empty;

                case COIN_MARKET.BITGET:
                    return symbol.Substring(symbol.Length - "usdt".Length).Equals("usdt") &&
                        symbol.Substring(0, "cmt_".Length).Equals("cmt_") ?
                        symbol.Replace("usdt", string.Empty).Replace("cmt_", string.Empty).ToUpper() :
                        string.Empty;

                case COIN_MARKET.BITZ:
                    return ConvertCoinNameByBitZ(symbol);

                case COIN_MARKET.BYBIT:
                    return symbol.Substring(symbol.Length - "USDT".Length).Equals("USDT") ? symbol.Replace("USDT", string.Empty) : string.Empty;

                case COIN_MARKET.FTX:
                    return symbol.Substring(symbol.Length - "-PERP".Length).Equals("-PERP") ? symbol.Replace("-PERP", string.Empty) : string.Empty;

                case COIN_MARKET.GATEIO:
                    return symbol.Substring(symbol.Length - "_USDT".Length).Equals("_USDT") ? symbol.Replace("_USDT", string.Empty) : string.Empty;

                case COIN_MARKET.HUOBI:
                    return symbol.Substring(symbol.Length - "-USDT".Length).Equals("-USDT") ? symbol.Replace("-USDT", string.Empty) : string.Empty;

                case COIN_MARKET.MXC:
                    return symbol.Substring(symbol.Length - "_USDT".Length).Equals("_USDT") ? symbol.Replace("_USDT", string.Empty) : string.Empty;

                case COIN_MARKET.OKEX:
                    return symbol.Substring(symbol.Length - "-USDT-SWAP".Length).Equals("-USDT-SWAP") ? symbol.Replace("-USDT-SWAP", string.Empty) : string.Empty;

                case COIN_MARKET.ZBG:
                    return symbol.Substring(symbol.Length - "_USDT".Length).Equals("_USDT") ? symbol.Replace("_USDT", string.Empty) : string.Empty;

                default:
                    throw new NotImplementedException("Not Support Market");
            }
        }

        private static string ConvertCoinNameByBitZ(string symbol)
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

        private static string ConvertContractIdByBitZ(COIN_TYPE cointype)
        {
            switch (cointype)
            {
                case COIN_TYPE.BTC:
                    return "101";

                case COIN_TYPE.ETH:
                    return "102";

                case COIN_TYPE.LTC:
                    return "105";

                case COIN_TYPE.BCH:
                    return "103";

                case COIN_TYPE.LINK:
                    return "121";

                default:
                    return string.Empty;
            }
        }
    }
}