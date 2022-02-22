namespace DataModels
{
    using Configuration;

    public class OrderResult
    {
        public COIN_MARKET BuyMarket { get; set; }

        public COIN_MARKET SellMarket { get; set; }
    }
}