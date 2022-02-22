using Microsoft.VisualStudio.TestTools.UnitTesting;
using Database;

namespace Database.Tests
{
    using Configuration;
    using Database;
    using Database.Interfaces;
    using DataModels;
    using Markets;
    using Markets.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    [TestClass()]
    public class DBControllerTests
    {
        [TestMethod()]
        public void LoadBalanceTest()
        {
            IDataBaseLoadController db = new DBController();

            Assert.IsNotNull(db.LoadBalance());
        }

        [TestMethod()]
        public void LoadCoreOptionsTest()
        {
            IDataBaseLoadController db = new DBController();

            Assert.IsNotNull(db.LoadCoreOptions());
        }

        [TestMethod()]
        public void LoadCoreOptionsByMarketAndCoinTest()
        {
            IDataBaseLoadController db = new DBController();

            Assert.IsNotNull(db.LoadCoreOptionsByMarket("BINANCE"));
        }

        [TestMethod()]
        public void RegisterMarketDatabaseTest()
        {
            IDBController db = new DBController();
            IMarket market = MarketFactory.CreateMarket(COIN_MARKET.BYBIT, new  Settings());
            db.RegisterMarketDatabase(market);

            Assert.IsTrue(true);
        }

        //[TestMethod()]
        //public void TempDeleteDBTest()
        //{
        //    string CONNECT_STRING = "server=db.reel.finance,1433;uid=reel;pwd=reelfinance102!;database=dbreel";

        //    SqlConnection conn = new SqlConnection(CONNECT_STRING);
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = conn;
        //    cmd.CommandType = System.Data.CommandType.Text;
        //    cmd.CommandText = "DELETE FROM MarginDev_Market_CoreOption_Advanced WHERE Market_Name=@Market_Name";

        //    cmd.Connection.Open();
        //    cmd.Parameters.AddWithValue("@Market_Name", "OKEX");
        //    cmd.ExecuteNonQuery();
        //    cmd.Dispose();
        //    cmd.Connection.Close();
        //    cmd.Connection.Dispose();

        //    Assert.IsTrue(true);
        //}
    }
}