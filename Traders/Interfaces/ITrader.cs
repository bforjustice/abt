namespace Traders.Interfaces
{
    using Configuration;
    using DataModels;
    using Markets.Interfaces;

    public interface ITrader : IRequsetOrder
    {
        //Related to Balance API
        Balance GetBalance(COIN_MARKET market);

        void SetBalance(COIN_MARKET market, Balance balance);

        // Related to Position API
        Position GetPosition(COIN_MARKET market, COIN_TYPE coinType);

        Position GetPosition(COIN_MARKET market, string symbol);

        void SetPosition(COIN_MARKET market, COIN_TYPE coinType, Position pos);

        POSITION_TYPE GetPositionType(COIN_MARKET market);

        // Publisher
        void SubscribeOrderInfo(COIN_MARKET market, IOrderInfoSubscriber subscriber);

        void UnsubscribeOrderInfo(COIN_MARKET market, IOrderInfoSubscriber subscriber);

        void SubscribeOrderBook(COIN_MARKET market, IOrderbookSubscriber subscriber);

        void UnsubscribeOrderBook(COIN_MARKET market, IOrderbookSubscriber subscriber);

        // Market info
        MARKET_STATE GetMarketState(COIN_MARKET market);

        //double GetMinTradeValue(COIN_MARKET market);

        void SetMarketState(COIN_MARKET market, MARKET_STATE state);

        void RegisterMarket(IMarket market);

        void UnregisterMarket(IMarket market);
    }
}