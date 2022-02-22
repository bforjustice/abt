using Configuration;
using DataModels;
using LogTrace.Interfaces;
using Traders.Interfaces;

namespace CalculationEngine.Algorithm
{
    public class AppliedAREEPAlg
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CalcLogger");

        public AppliedAREEPAlg()
        {
        }

        public Order Calc(ITrader trader, Order originOrder)
        {
            double areeRatio = 1.2;

            Order newOrder = originOrder.Clone();

            Position pos = trader.GetPosition(newOrder.Market, newOrder.Symbol);

            POSITION_SIDE orderPosSide = newOrder.Side.Equals(ORDER_SIDE.buy) ? POSITION_SIDE.LONG : POSITION_SIDE.SHORT;
            if (pos.PositionAmount(orderPosSide) == 0)
            {
                newOrder.Direction = ORDER_DIRECTION.OPEN;
                myLogger.Info($"Position 0 Order Market : {originOrder.Market.ToString()} Posotion Type {newOrder.Direction}");
                return newOrder;
            }

            if (trader.GetPositionType(originOrder.Market).Equals(POSITION_TYPE.ONEWAY))
            {
                newOrder.Direction = ORDER_DIRECTION.OPEN;
                myLogger.Info($"Oneway Market Order Market : {originOrder.Market.ToString()} Posotion Type {newOrder.Direction}");
                return newOrder;
            }

            // Applied AREE
            double amountRatio = (newOrder.Quantity * newOrder.OrderPrice) / pos.PositionAmount(orderPosSide);

            if (amountRatio <= areeRatio)
            {
                newOrder.Direction = ORDER_DIRECTION.CLOSE;
            }
            else
            {
                newOrder.Side = newOrder.Side.Equals(ORDER_SIDE.buy) ? ORDER_SIDE.sell : ORDER_SIDE.buy;
                newOrder.Direction = ORDER_DIRECTION.OPEN;
            }

            myLogger.Info($"Applied AREE P Order Market : {originOrder.Market.ToString()} Posotion Type {newOrder.Direction}");
            return newOrder;
        }
    }
}