namespace CalculationEngine.Algorithm
{
    using CalculationEngine.Algorithm.Interfaces;
    using Configuration;
    using DataModels;
    using LogTrace.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using Traders.Interfaces;

    public class FindBestPriceForReOrder : IUpdatedOrderbook
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CalcLogger");

        private readonly string TAG = "FindBestPriceForReOrder";
        private OrderBooks myLatestOrderbooks;

        public FindBestPriceForReOrder()
        {
        }

        public Order Calc(ITrader trader, ORDER_SIDE orderSide, double remainQty, Order originOrder)
        {
            return FindBestMarket(trader, orderSide, remainQty, originOrder);
        }

        public void UpdateOrderbook(OrderBooks orderBooks)
        {
            this.myLatestOrderbooks = orderBooks;
        }

        private double GetAveragePrice(ORDER_SIDE orderSide, double remainQty)
        {
            IList<OrderBook> orderBooks = orderSide.Equals(ORDER_SIDE.buy) ?
                this.myLatestOrderbooks.SellOrderbooks : this.myLatestOrderbooks.BuyOrderbooks;

            double totalQty = 0;
            double totalAmount = 0;

            foreach (OrderBook orderBook in orderBooks)
            {
                for (int idx = 0; idx < orderBook.DepthQuantity(orderSide).Count(); ++idx)
                {
                    totalQty += orderBook.DepthQuantity(orderSide)[idx];
                    totalAmount += orderBook.DepthPrice(orderSide)[idx] * orderBook.DepthQuantity(orderSide)[idx];

                    if (totalQty >= remainQty)
                    {
                        break;
                    }
                }
            }

            return totalAmount / totalQty; // 평균가격 (마켓별로 체결 평균가 비교를 위해)
        }

        private Order FindBestMarket(ITrader trader, ORDER_SIDE orderSide, double remainQty, Order originOrder)
        {
            COIN_MARKET bestMarket = COIN_MARKET.BINANCE;
            double bestPrice = orderSide.Equals(ORDER_SIDE.buy) ? double.MaxValue : 0;
            IList<OrderBook> orderBooks = orderSide.Equals(ORDER_SIDE.buy) ?
                this.myLatestOrderbooks.SellOrderbooks : this.myLatestOrderbooks.BuyOrderbooks;

            bool isFind = false;
            foreach (OrderBook orderBook in orderBooks)
            {
                //myLogger.Error($"check best, market = {coinMarket}");
                //myLogger.Error($"check best, averagePrice = {tradablePriceList[0, (int)coinMarket]}, bestPrice = {bestPrice}");
                //myLogger.Error($"check best, Balance_USDT = {Balance.Balance_USDT[(int)coinMarket]}, remain = {tradablePriceList[1, (int)coinMarket] * remainQty}");
                Balance balance = trader.GetBalance(orderBook.Market);

                if (balance.Balance_USDT >= orderBook.DepthPrice(orderSide)[0] * remainQty)
                {
                    double avgPrice = this.GetAveragePrice(orderSide, remainQty);

                    if ((orderSide.Equals(ORDER_SIDE.buy) && avgPrice < bestPrice)
                        || (orderSide.Equals(ORDER_SIDE.sell) && avgPrice > bestPrice))
                    {
                        bestPrice = orderBook.DepthPrice(orderSide)[0];
                        bestMarket = orderBook.Market;
                        myLogger.Warn($"bestMarket changed, {bestMarket}, bestPrice = {bestPrice}, Remain Qty : {remainQty}");
                        isFind = true;
                    }
                }
            }

            if (isFind)
            {
                Order newOrder = originOrder.Clone();
                newOrder.OrderPrice = bestPrice;
                newOrder.Market = bestMarket;
                newOrder.Side = orderSide;
                newOrder.Type = ORDER_TYPE.limit;
                newOrder.Quantity = remainQty;
                newOrder.Symbol = trader.SymbolExchange(originOrder.Market, bestMarket, originOrder.Symbol);

                myLogger.Warn($"Return ReOrder Info : \n{newOrder.ToString()}");
                return newOrder;
            }
            else
            {
                myLogger.Warn($"Return Original Info : \n{originOrder.ToString()}");
                Order newOrder = originOrder.Clone();
                newOrder.Quantity = remainQty;
                return originOrder;
            }
        }
    }
}