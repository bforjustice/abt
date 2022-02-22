namespace CalculationEngine.Strategies.SubStrategies
{
    using CalculationEngine.Strategies.SubStrategies.Interfaces;
    using DataModels;
    using Traders.Interfaces;

    public class OrderChecker : OrdererBase, IOrderer
    {
        public OrderChecker(ITrader trader, OrderInfo orderInfo, PendingInfo pendingInfo, ManageOrderCycle orderCycle, Order originOrder)
            : base(trader, orderInfo, pendingInfo, orderCycle, originOrder)
        {
        }

        public bool DoWork()
        {
            myLogger.Info($"\nOpen Order Info :: \n{this.myOrderInfo.ToString()}");
            this.myTrader.RequestOrderInfo(this.myOrderInfo).WaitOne();
            return true;
        }
    }
}