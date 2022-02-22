namespace Markets.Controls
{
    using DataModels;
    using System.Collections.Generic;

    public class EmptyResponseControl : ResponseControlBase
    {
        public override Balance ParseBalance(Settings settings, APIResult result)
        {
            return Balance.Empty();
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            return new OrderInfo();
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            return "0";
        }

        public override OrderBook ParseOrderBook(Settings settings, APIResult result)
        {
            return OrderBook.Empty();
        }

        public override OrderBook ParseOrderBookByWebSocket(Settings settings, APIResult result)
        {
            return OrderBook.Empty();
        }

        public override OrderInfo ParseOrderInfo(Settings settings, APIResult result)
        {
            return new OrderInfo();
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            return new OrderInfo();
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            return new List<Position>() { Position.Empty() };
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            return new Tickers();
        }
    }
}