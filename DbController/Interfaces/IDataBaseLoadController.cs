using Configuration;
using DataModels;
using System.Collections.Generic;
using Traders.Interfaces;

namespace Database.Interfaces
{
    public interface IDataBaseLoadController
    {
        IDictionary<COIN_MARKET, Balance> LoadBalance();

        IDictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>> LoadPosition();

        Dictionary<string, string> LoadCoreOptions();

        Dictionary<string, string> LoadCoreOptionsByMarket(string marketName);

        void LoadBalanceAndPosition(ITrader trader);
    }
}