namespace CalculationEngine.Strategies.SubStrategies
{
    using DataModels;
    using LogTrace.Interfaces;
    using Traders.Interfaces;

    public abstract class OrdererBase
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CalcLogger");

        protected ITrader myTrader;

        protected OrderInfo myOrderInfo;

        protected PendingInfo myPendingInfo;

        protected ManageOrderCycle myOrderCycle;

        protected Order myOriginOrder;

        public OrdererBase(ITrader trader, OrderInfo orderInfo, PendingInfo pendingInfo, ManageOrderCycle orderCycle, Order originOrder)
        {
            this.myOrderInfo = orderInfo;
            this.myTrader = trader;
            this.myPendingInfo = pendingInfo;
            this.myOrderCycle = orderCycle;
            this.myOriginOrder = originOrder;
        }
    }
}