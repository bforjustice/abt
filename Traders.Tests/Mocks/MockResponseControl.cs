using DataModels;
using Markets.Controls;
using System;
using System.Collections.Generic;

namespace Traders.Tests.Mocks
{
    public class MockResponseControl : ResponseControlBase
    {
        public override Balance ParseBalance(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override OrderBook ParseOrderBook(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override OrderBook ParseOrderBookByWebSocket(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override OrderInfo ParseOrderInfo(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }
    }
}