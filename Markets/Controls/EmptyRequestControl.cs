namespace Markets.Controls
{
    using Configuration;
    using Markets.Interfaces;
    using System.Threading;

    public class EmptyRequestControl : RequestControlBase
    {
        public EmptyRequestControl(IRequestFactory factory)
            : base(factory)
        {
        }

        public override AutoResetEvent GetBalance(int tId)
        {
            return new AutoResetEvent(true);
        }

        public override AutoResetEvent GetPosition(int tId)
        {
            return new AutoResetEvent(true);
        }

        public override AutoResetEvent GetTickers(int tId)
        {
            return new AutoResetEvent(true);
        }

        public override AutoResetEvent GetOrderbook(string symbol, int tId)
        {
            return new AutoResetEvent(true);
        }

        public override AutoResetEvent PlaceOrder(string symbol, double price, double qty, ORDER_SIDE orderSide, ORDER_DIRECTION orderDirection, ORDER_TYPE orderType, int tId)
        {
            return new AutoResetEvent(true);
        }

        public override AutoResetEvent CancelOrder(string symbol, string orderId, int tId)
        {
            return new AutoResetEvent(true);
        }

        public override AutoResetEvent SetLeverage(string symbol, int leverage, int tId)
        {
            return new AutoResetEvent(true);
        }

        public override AutoResetEvent GetOrderInfo(string symbol, string orderId, int tId)
        {
            return new AutoResetEvent(true);
        }
    }
}