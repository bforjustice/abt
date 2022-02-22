namespace CalculationEngine.Algorithm
{
    using DataModels;
    using LogTrace.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Traders.Interfaces;

    public class AlignOrderUnit
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CalcLogger");

        public AlignOrderUnit()
        {
        }

        public Data Calc(ITrader trader, Data originOrder)
        {
            double orderUnit = double.MaxValue;

            if (originOrder.BuyMarketOrderUnit > originOrder.SellMarketOrderUnit)
            {
                orderUnit = originOrder.BuyMarketOrderUnit;
            }
            else
            {
                orderUnit = originOrder.SellMarketOrderUnit;
            }

            originOrder.BuyMarketOrderUnit = orderUnit;
            originOrder.SellMarketOrderUnit = orderUnit;

            double oldQuantity = originOrder.Quantity;
            originOrder.Quantity = Math.Floor(originOrder.Quantity / orderUnit) * orderUnit;

            myLogger.Info($"Common OrderUnit : {orderUnit} Origin Qty : {oldQuantity} Modified Qty : {originOrder.Quantity.ToString()}");

            return originOrder;
        }
    }
}
