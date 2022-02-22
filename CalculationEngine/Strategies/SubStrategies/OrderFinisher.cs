namespace CalculationEngine.Strategies.SubStrategies
{
    using CalculationEngine.Strategies.SubStrategies.Interfaces;
    using Configuration;
    using DataModels;
    using Traders.Interfaces;

    public class OrderFinisher : OrdererBase, IOrderer
    {
        public OrderFinisher(ITrader trader, OrderInfo orderInfo, PendingInfo pendingInfo, ManageOrderCycle orderCycle, Order originOrder)
            : base(trader, orderInfo, pendingInfo, orderCycle, originOrder)
        {
        }

        public bool DoWork()
        {
            myLogger.Info($"Finish Order Info :: {this.myOrderInfo.ToString()}");
            this.myTrader.SetMarketState(myOrderInfo.Market, MARKET_STATE.NORMAL);
            return true;
        }
    }
}