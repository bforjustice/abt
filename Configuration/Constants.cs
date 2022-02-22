namespace Configuration
{
    using System;

    public static class Constants
    {
        public static int MARKET_COUNT { get; set; } = Enum.GetNames(typeof(COIN_MARKET)).Length;
        public static int POSITION_COUNT { get; set; } = Enum.GetNames(typeof(POSITION_SIDE)).Length;
        public static int COIN_COUNT { get; set; } = Enum.GetNames(typeof(COIN_TYPE)).Length;
        public static int ORDERBOOK_SIDE_COUNT { get; set; } = Enum.GetNames(typeof(ORDERBOOK_SIDE)).Length;
        public static int TRADING_TYPE_COUNT { get; set; } = Enum.GetNames(typeof(TRADING_TYPE)).Length;
        public static string[] MARKET_FUCK_STRING { get; } = { "(JBN)", "(JHB)", "(JOK)", "(JBB)", "(JBG)" };

        public static long[] LastOrderTime { get; set; } = new long[MARKET_COUNT];
        public static long[] LastFuckTime { get; set; } = new long[MARKET_COUNT];
        public static bool[] MarketTradable { get; set; } = new bool[MARKET_COUNT];

        public static bool DEBUG { get; set; } = false;
        public static int ORDERBOOK_MAX_SIZE { get; set; } = 20;
        public static int ORDERBOOK_SIZE { get; set; } = 4;
        public static int ORDERBOOK_TIME_QUEUE_SIZE { get; set; } = 10;

        public static int balanceTimeout = 2000;

        public static long[] tradingTime = new long[COIN_COUNT];
        public static long[] handlePendingTime = new long[COIN_COUNT];
        public static double[,] lastPrice = new double[MARKET_COUNT, COIN_COUNT];

        public static bool[,] isSupport = new bool[MARKET_COUNT, COIN_COUNT];
        //public static double[,] minBidAmount = new double[MARKET_COUNT, COIN_COUNT];

        public static string[] API_KEY = new string[MARKET_COUNT];
        public static string[] SECRET_KEY = new string[MARKET_COUNT];
    }
}