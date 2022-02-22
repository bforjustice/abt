namespace Database.Interfaces
{
    using Configuration;
    using DataModels;
    using System.Collections.Generic;

    public enum LOG_TYPE { MARGIN = 0, ERROR, EXCEPTION, FAIL_HANDLE_PENDING };

    public interface IDataBaseStoreController
    {
        void StoreBalanceDataAndPosition(IList<Balance> balances, IList<Position> pos);

        void StoreRealTimeData(string symbol, string lowerMarket, string upperMarket, double lowerPrice, double upperPrice, double lowerQty, double upperQty);

        void StorePLDailyLogDataToDB(IList<Balance> balances, IList<Position> positions);

        void StoreCompletedTradeLog(LOG_TYPE logType, COIN_TYPE coinType, PendingInfo[] finishedInfos, string memo);

        void StoreCompletedTradeLog(LOG_TYPE logType, COIN_TYPE coinType, IDictionary<string, string> buySideTradeInfo, IDictionary<string, string> sellSideTradeInfo, double usdtMargin, string memo);

        void StoreExceptionInfo(COIN_TYPE coinType, string memo);
    }
}