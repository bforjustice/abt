namespace Traders.Interfaces
{
    using Configuration;
    using DataModels;
    using System.Collections.Generic;
    using System.Threading;

    public interface IRequsetOrder
    {
        AutoResetEvent RequestOrder(Order order);

        AutoResetEvent RequestOrderInfo(OrderInfo orders);

        List<AutoResetEvent> RequestOrderInfo(IList<OrderInfo> orders);

        AutoResetEvent RequestCancelOrder(OrderInfo orderInfo);

        string SymbolExchange(COIN_MARKET srcMarket, COIN_MARKET dstMarket, string originSymbol);
    }
}