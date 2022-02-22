namespace CalculationEngine.Strategies.SubStrategies.Interfaces
{
    using DataModels;
    using System;
    using Traders.Interfaces;

    public static class OrdererFactory
    {
        public enum ORDER_INFO_STATE
        {
            INIT,
            SUCCESS,
            PAITAIAL_SUCCESS,
            NEED_CANCELED,
            NEED_ORDER_CHECK,
            RETRY_ORDER,
            ORDER_FAILED,
        };

        public static IOrderer CreateOrderer(ORDER_INFO_STATE state, ITrader trader, OrderInfo orderInfo, PendingInfo pendingInfo, ManageOrderCycle orderCycle, Order originOrder)
        {
            switch (state)
            {
                case ORDER_INFO_STATE.SUCCESS:
                case ORDER_INFO_STATE.ORDER_FAILED:
                    return new OrderFinisher(trader, orderInfo, pendingInfo, orderCycle, originOrder);

                case ORDER_INFO_STATE.NEED_CANCELED:
                    return new CancelOrderer(trader, orderInfo, pendingInfo, orderCycle, originOrder);

                case ORDER_INFO_STATE.RETRY_ORDER:
                    return new ReOrderer(trader, orderInfo, pendingInfo, orderCycle, originOrder);

                case ORDER_INFO_STATE.NEED_ORDER_CHECK:
                    return new OrderChecker(trader, orderInfo, pendingInfo, orderCycle, originOrder);

                default:
                    throw new NotImplementedException("NOT SUPPORTED STATE");
            }
        }
    }
}