using Configuration;
using Markets.Controls;
using Markets.Interfaces;
using System;
using System.Threading;

namespace Traders.Tests.Mocks
{
    public class MockRequestControl : RequestControlBase
    {
        public MockRequestControl(IRequestFactory factory)
            : base(factory)
        {
        }

        public override AutoResetEvent CancelOrder(string symbol, string orderId, int tId)
        {
            throw new NotImplementedException();
        }

        public override AutoResetEvent GetBalance(int tId)
        {
            throw new NotImplementedException();
        }

        public override AutoResetEvent GetOrderbook(string symbol, int tId)
        {
            throw new NotImplementedException();
        }

        public override AutoResetEvent GetOrderInfo(string symbol, string orderId, int tId)
        {
            throw new NotImplementedException();
        }

        public override AutoResetEvent GetPosition(int tId)
        {
            throw new NotImplementedException();
        }

        public override AutoResetEvent GetTickers(int tId)
        {
            throw new NotImplementedException();
        }

        public override AutoResetEvent PlaceOrder(string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, int tId)
        {
            throw new NotImplementedException();
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            throw new NotImplementedException();
        }
    }
}