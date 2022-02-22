namespace DataModels
{
    using Configuration;

    public class CoinParamGroup
    {
        private COIN_TYPE myCoinType;

        public CoinParamGroup(COIN_TYPE type)
        {
            this.myCoinType = type;
        }

        public double OrderUnit { get; set; }

        public double MinTradeValue { get; set; }

        public int DecimalLength { get; set; }

        // Will be removed
        public bool IsSupport { get; set; }

        public double PriceDiffThreshold { get; set; }

        public double MarginThreshold { get; set; }

        public double Bound { get; set; }
    }
}