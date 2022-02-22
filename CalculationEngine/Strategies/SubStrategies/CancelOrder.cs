namespace CalculationEngine.Strategies.SubStrategies
{
    using CalculationEngine.Strategies.SubStrategies.Interfaces;
    using DataModels;
    using Traders.Interfaces;

    public class CancelOrderer : OrdererBase, IOrderer
    {
        public CancelOrderer(ITrader trader, OrderInfo orderInfo, PendingInfo pendingInfo, ManageOrderCycle orderCycle, Order originOrder)
            : base(trader, orderInfo, pendingInfo, orderCycle, originOrder)
        {
        }

        public bool DoWork()
        {
            myLogger.Info($"Cancel Order :: {this.myOrderInfo.ToString()}");
            this.myTrader.RequestCancelOrder(this.myOrderInfo).WaitOne();
            return true;
        }
    }
}