using Markets.Interfaces;

namespace Database.Interfaces
{
    public interface IDBController : IDataBaseStoreController, IDataBaseLoadController
    {
        void RegisterMarketDatabase(IMarket market);
    }
}