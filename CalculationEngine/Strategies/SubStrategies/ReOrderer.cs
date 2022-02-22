namespace CalculationEngine.Strategies.SubStrategies
{
    using CalculationEngine.Strategies.SubStrategies.Interfaces;
    using Configuration;
    using DataModels;
    using Traders.Interfaces;

    public class ReOrderer : OrdererBase, IOrderer
    {
        public ReOrderer(ITrader trader, OrderInfo orderInfo, PendingInfo pendingInfo, ManageOrderCycle orderCycle, Order originOrder)
            : base(trader, orderInfo, pendingInfo, orderCycle, originOrder)
        {
        }

        public bool DoWork()
        {
            myLogger.Info($"Reorder :: \n{this.myOrderInfo.ToString()}\n");

            // avg Price 와 best market, best price를 구함
            Order retryOrder = AlgManager.Instance.FindBestPriceForReorder(this.myTrader, this.myOrderInfo.Side, this.myOrderInfo.RemainQty, this.myOriginOrder);

            if (!this.myOrderInfo.Market.Equals(retryOrder.Market))
            {
                myLogger.Info($"Restore Market State :: \nFrom : {this.myOriginOrder.Market.ToString()} To : {retryOrder.Market.ToString()}\n");
                this.myTrader.SetMarketState(myOrderInfo.Market, MARKET_STATE.NORMAL);
                this.myTrader.UnsubscribeOrderBook(myOrderInfo.Market, this.myOrderCycle);
                this.myTrader.UnsubscribeOrderInfo(myOrderInfo.Market, this.myOrderCycle);
            }

            this.myTrader.SetMarketState(retryOrder.Market, MARKET_STATE.RECOVERY);

            this.myTrader.SubscribeOrderBook(retryOrder.Market, this.myOrderCycle);
            this.myTrader.SubscribeOrderInfo(retryOrder.Market, this.myOrderCycle);

            this.myTrader.RequestOrder(retryOrder).WaitOne();
            this.myOriginOrder = retryOrder;

            myLogger.Info($"Retry Order Info :: \n{this.myOriginOrder.ToString()}\n");

            return true;
        }
    }
}