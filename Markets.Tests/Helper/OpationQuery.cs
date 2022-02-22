namespace Markets.Tests.Helper
{
    using Configuration;
    using Database;
    using Database.Interfaces;
    using DataModels;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using Traders.Interfaces;

    public class OpationQuery
    {
        private readonly string CONNECT_STRING = "server=db.reel.finance,1433;uid=reel;pwd=reelfinance102!;database=dbreel";

        public void LoadBalanceAndPosition(ITrader trader)
        {
            IDBController dbCtrl = new DBController();
            dbCtrl.LoadBalanceAndPosition(trader);
        }

        public Settings CreateSettingValues(COIN_MARKET market)
        {
            IDBController dbCtrl = new DBController();
            return Settings.CreateMarketSettings(dbCtrl.LoadCoreOptionsByMarket(market.ToString()));
        }

        private Dictionary<string, string> loadCoreOption(COIN_TYPE coinType)
        {
            Dictionary<string, string> optionDic = new Dictionary<string, string>();

            SqlConnection conn = new SqlConnection(CONNECT_STRING);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandText = "MarginDev_CoreOption_Select";
            try
            {
                conn.Open();

                DataSet ds = new DataSet();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds);

                DataTable commonTable = ds.Tables[0];
                DataTable marketTable = ds.Tables[1];
                DataTable coinTable = ds.Tables[2];

                optionDic.Add("PendingTimeInSecond", commonTable.Rows[0]["AfterTrade_Standby_Sec"].ToString());
                optionDic.Add("OrderbookPeriod", commonTable.Rows[0]["Orderbook_Period"].ToString());
                optionDic.Add("OrderbookLimit", commonTable.Rows[0]["Orderbook_Limit"].ToString());
                for (int i = 0; i < marketTable.Rows.Count; i++)
                {
                    string marketName = marketTable.Rows[i]["Market_Name"].ToString();

                    optionDic.Add($"TradeFreq_{marketName}", marketTable.Rows[i]["TradeFreq"].ToString());
                    optionDic.Add($"{marketName}_COIN_Enable", marketTable.Rows[i][$"{coinType.ToString()}_Enable"].ToString());
                    optionDic.Add($"{marketName}_Enable", marketTable.Rows[i]["Enable"].ToString());
                    optionDic.Add($"MakerFee_{marketName}", marketTable.Rows[i]["Maker_Fee"].ToString());
                    optionDic.Add($"TakerFee_{marketName}", marketTable.Rows[i]["Taker_Fee"].ToString());
                    //optionDic.Add($"FeeAuto_{marketName}", marketTable.Rows[i]["Fee_AutoMode"].ToString());

                    optionDic.Add($"{marketName}_Order_Unit", marketTable.Rows[i][$"{coinType}_Ask_Unit"].ToString());
                    optionDic.Add($"{marketName}_Min_Trade_Value", marketTable.Rows[i][$"{coinType}_Min_Bid_Amount"].ToString());

                    optionDic.Add($"{marketName}_Leverage", marketTable.Rows[i]["Leverage"].ToString());
                    optionDic.Add($"{marketName}_API_KEY", marketTable.Rows[i]["API_KEY"].ToString());
                    optionDic.Add($"{marketName}_SECRET_KEY", marketTable.Rows[i]["SECRET_KEY"].ToString());
                }

                optionDic.Add("Enabled", coinTable.Rows[(int)coinType]["Toggle_OnOff"].ToString());
                optionDic.Add("PriceDiffThreshold", coinTable.Rows[(int)coinType]["PriceDiffThreshold"].ToString());
                optionDic.Add("Bound", coinTable.Rows[(int)coinType]["Quotation"].ToString());
                optionDic.Add("MarginThreshold", coinTable.Rows[(int)coinType]["Min_Devouring"].ToString());

                cmd.Dispose();
            }
            catch (Exception err)
            {
                return null;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return optionDic;
        }
    }
}