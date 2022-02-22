namespace Database
{
    using Common;
    using Configuration;
    using Database.Interfaces;
    using DataModels;
    using LogTrace.Interfaces;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;
    using Traders.Interfaces;

    public class DBController : IDBController
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("DatabaseLogger");

        private readonly string CONNECT_STRING = "server=db.reel.finance,1433;uid=reel;pwd=reelfinance102!;database=dbreel";

        public void StoreBalanceDataAndPosition(IList<Balance> balances, IList<Position> pos)
        {
            this.updateBalance(balances);
            this.updatePosition(pos);
        }

        public void StorePLDailyLogDataToDB(IList<Balance> balances, IList<Position> positions)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_PLDailyLog_Insert");

            try
            {
                cmd.Connection.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Date_Time", TimeManager.SyncronizedTime);

                double totalUSDT = 0;

                foreach (Balance bal in balances)
                {
                    totalUSDT += bal.TotalBalance_USDT();
                }

                cmd.Parameters.AddWithValue("@TotalProperty_USDT", totalUSDT);

                foreach (var coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    double sum = 0;
                    foreach (var coinMarket in Enum.GetValues(typeof(COIN_MARKET)))
                    {
                        Position pos = null;

                        if (this.FindPosition(positions, (COIN_MARKET)coinMarket, (COIN_TYPE)coinType, out pos))
                        {
                            sum += pos.Total;
                        }
                    }
                    cmd.Parameters.AddWithValue($"@Unrealized_{coinType.ToString()}", CommonApi.cutDecimalNumber(sum, 4));
                }

                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Error(err.ToString());
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        public void StoreRealTimeData(string symbol, string lowerMarket, string upperMarket, double lowerPrice, double upperPrice, double lowerQty, double upperQty)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_RealTime_Update");

            try
            {
                cmd.Connection.Open();
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@Date_Time", TimeManager.SyncronizedTime);
                cmd.Parameters.AddWithValue("@Lower", lowerPrice);
                cmd.Parameters.AddWithValue("@Upper", upperPrice);
                cmd.Parameters.AddWithValue("@Market_Name_Upper", upperMarket);
                cmd.Parameters.AddWithValue("@Market_Name_Lower", lowerMarket);
                cmd.Parameters.AddWithValue("@Quantity_Upper", upperQty);
                cmd.Parameters.AddWithValue("@Quantity_Lower", lowerQty);
                cmd.Parameters.AddWithValue("@Coin_Name", symbol);
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        public Dictionary<string, string> LoadCoreOptions()
        {
            //return this.legacyLoadCoreOptionData();
            return this.LoadCoreOptionData();
        }

        public void LoadBalanceAndPosition(ITrader trader)
        {
            IDictionary<COIN_MARKET, Balance> balances = this.LoadBalance();
            IDictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>> positions = this.LoadPosition();

            foreach (COIN_MARKET market in balances.Keys)
            {
                if (!trader.GetMarketState(market).Equals(MARKET_STATE.EMPTY))
                {
                    trader.SetBalance(market, balances[market]);
                }
            }

            foreach (COIN_MARKET market in positions.Keys)
            {
                foreach (COIN_TYPE coinType in positions[market].Keys)
                {
                    if (!trader.GetMarketState(market).Equals(MARKET_STATE.EMPTY))
                    {
                        trader.SetPosition(market, coinType, positions[market][coinType]);
                    }
                }
            }
        }

        public IDictionary<COIN_MARKET, Balance> LoadBalance()
        {
            IDictionary<COIN_MARKET, Balance> res = new Dictionary<COIN_MARKET, Balance>();
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Balance_Select_Advanced");
            DataTable table = new DataTable();

            try
            {
                cmd.Connection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(table);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Balance balance = new Balance();
                    COIN_MARKET currentMarket;
                    if (!Enum.TryParse<COIN_MARKET>(table.Rows[i]["Market_Name"].ToString(), out currentMarket))
                    {
                        continue;
                    }

                    balance.Market = currentMarket;
                    balance.Balance_USDT = Convert.ToDouble(table.Rows[i]["USDT"]);
                    balance.BalanceLocked_USDT = Convert.ToDouble(table.Rows[i]["USDT_LOCKED"]);

                    if (res.ContainsKey(currentMarket))
                    {
                        continue;
                    }

                    res.Add(currentMarket, balance);
                }

                cmd.Dispose();
                return res;
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
                return null;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        private void updateBalance(IList<Balance> balances)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Balance_Advanced_Update_Bulk");

            try
            {
                DataTable table = new DataTable();
                table.Clear();

                table.Columns.Add("Market_Name");
                table.Columns.Add("Date_Time", typeof(DateTime));
                table.Columns.Add("USDT");
                table.Columns.Add("USDT_LOCKED");

                foreach (Balance balance in balances)
                {
                    DataRow row = table.NewRow();
                    row["Market_Name"] = balance.Market.ToString();
                    row["Date_Time"] = TimeManager.SyncronizedTime;
                    row["USDT"] = balance.TotalBalance_USDT();
                    row["USDT_LOCKED"] = balance.BalanceLocked_USDT;
                    table.Rows.Add(row);
                }

                cmd.Connection.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@BalanceTable", table);
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Fatal("updateBalance exception, " + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        public void updatePosition(IList<Position> positions)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Position_Update_Bulk");

            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("Market_Name");
                table.Columns.Add("Coin_Name");
                table.Columns.Add("Quantity");
                table.Columns.Add("Price");
                table.Columns.Add("PNL");
                table.Columns.Add("Side");
                table.Columns.Add("Leverage");

                foreach (COIN_MARKET market in Enum.GetValues(typeof(COIN_MARKET)))
                {
                    foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                    {
                        foreach (POSITION_SIDE side in Enum.GetValues(typeof(POSITION_SIDE)))
                        {
                            Position pos = null;
                            if (this.FindPosition(positions, market, coinType, out pos))
                            {
                                DataRow row = table.NewRow();
                                row["Market_Name"] = market.ToString();
                                row["Coin_Name"] = coinType.ToString();
                                row["Quantity"] = Math.Abs(pos.GetQuantity(side));
                                row["Price"] = pos.GetPrice(side);
                                row["PNL"] = pos.GetPNL(side);
                                row["Side"] = side.ToString();
                                row["Leverage"] = pos.GetLeverage(side);
                                table.Rows.Add(row);
                            }
                        }
                    }
                }

                cmd.Connection.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PositionTable", table);
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Fatal("updatePosition exception, " + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        private SqlCommand createConnectionAndCommander(CommandType type, string commandText)
        {
            SqlConnection conn = new SqlConnection(CONNECT_STRING);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = type;
            cmd.CommandText = commandText;

            return cmd;
        }

        private bool FindPosition(IList<Position> positions, COIN_MARKET market, COIN_TYPE type, out Position res)
        {
            foreach (Position pos in positions)
            {
                if (pos.Market.Equals(market) && pos.CoinType.Equals(type))
                {
                    res = pos;
                    return true;
                }
            }

            res = null;
            return false;
        }

        public IDictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>> LoadPosition()
        {
            IDictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>> positions =
                this.CreateDefaultPositionSet();

            foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
            {
                SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_BalanceAndPosition_Select");
                cmd.Parameters.AddWithValue("Coin_Name", coinType.ToString());
                cmd.Connection.Open();

                DataSet ds = new DataSet();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds);

                DataTable positionTable = ds.Tables[1];

                //for (int idx = 0; idx < positionTable.Rows.Count; ++idx)
                foreach (DataRow row in positionTable.Rows)
                {
                    COIN_TYPE coinName = (COIN_TYPE)Enum.Parse(typeof(COIN_TYPE), row.Field<string>("Coin_Name"));
                    COIN_MARKET market;
                    if (!Enum.TryParse<COIN_MARKET>(row.Field<string>("Market_Name"), out market))
                    {
                        continue;
                    }

                    Position position = positions[market][coinName];

                    POSITION_SIDE side = (POSITION_SIDE)Enum.Parse(typeof(POSITION_SIDE), row.Field<string>("Side"));

                    position.SetQuantity(side, Math.Abs(row.Field<double>("Quantity")));
                    position.SetPrice(side, row.Field<double>("Price"));
                    position.SetPNL(side, row.Field<double>("PNL"));
                    position.SetLeverage(side, row.Field<double>("Leverage"));

                    if (!positions.ContainsKey(market))
                    {
                        positions.Add(market, new Dictionary<COIN_TYPE, Position>());
                    }

                    if (!positions[market].ContainsKey(coinName))
                    {
                        positions[market].Add(coinName, Position.Empty());
                    }

                    positions[market][coinName] = position;
                }

                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }

            return positions;
        }

        public void StoreCompletedTradeLog(LOG_TYPE logType, COIN_TYPE coinType, IDictionary<string, string> buySideTradeInfo, IDictionary<string, string> sellSideTradeInfo, double usdtMargin, string memo)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Log_Insert");

            try
            {
                cmd.Connection.Open();
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@Coin_Name", coinType.ToString());
                cmd.Parameters.AddWithValue("@Log_Type", (int)logType);
                cmd.Parameters.AddWithValue("@Date_Time", TimeManager.SyncronizedTime);
                cmd.Parameters.AddWithValue("@Margin_USDT", usdtMargin);

                foreach (var keyItem in buySideTradeInfo)
                {
                    cmd.Parameters.AddWithValue(keyItem.Key, keyItem.Value);
                }

                foreach (var keyItem in sellSideTradeInfo)
                {
                    cmd.Parameters.AddWithValue(keyItem.Key, keyItem.Value);
                }

                cmd.Parameters.AddWithValue("@Memo", memo);
                cmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                myLogger.Debug("error while DBController\n" + error.ToString());
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public void StoreCompletedTradeLog(LOG_TYPE logType, COIN_TYPE coinType, PendingInfo[] finishedInfos, string memo)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Log_Insert");
            cmd.Connection.Open();

            try
            {
                cmd.Connection.Open();
                cmd.Parameters.Clear();

                PendingInfo buyInfo = finishedInfos[(int)ORDER_SIDE.buy];
                PendingInfo sellInfo = finishedInfos[(int)ORDER_SIDE.sell];

                double usdtMargin = (sellInfo.TotalFilledQty * sellInfo.AvgPriceForMargin) - (buyInfo.TotalFilledQty * buyInfo.AvgPriceForMargin);

                cmd.Parameters.AddWithValue("@Coin_Name", coinType.ToString());
                cmd.Parameters.AddWithValue("@Log_Type", (int)logType);
                cmd.Parameters.AddWithValue("@Date_Time", TimeManager.SyncronizedTime);
                cmd.Parameters.AddWithValue("@Margin_USDT", usdtMargin);

                cmd.Parameters.AddWithValue("@Buy_Original_Market_Name", buyInfo.InitialMarket.ToString());
                cmd.Parameters.AddWithValue("@Buy_Market_Name", buyInfo.MostMarket.ToString());
                cmd.Parameters.AddWithValue("@Buy_Coin_Quantity", CommonApi.cutDecimalNumber(buyInfo.TotalFilledQty, 4));
                cmd.Parameters.AddWithValue("@Buy_Coin_Price", buyInfo.AvgPriceOriginal);
                cmd.Parameters.AddWithValue("@Buy_Coin_Original_Price", buyInfo.OriginalBuyPrice);
                cmd.Parameters.AddWithValue("@Buy_Trade_Type", buyInfo.TradeType);

                cmd.Parameters.AddWithValue("@Sell_Original_Market_Name", sellInfo.InitialMarket.ToString());
                cmd.Parameters.AddWithValue("@Sell_Market_Name", sellInfo.MostMarket.ToString());
                cmd.Parameters.AddWithValue("@Sell_Coin_Quantity", CommonApi.cutDecimalNumber(sellInfo.TotalFilledQty, 4));
                cmd.Parameters.AddWithValue("@Sell_Coin_Price", sellInfo.AvgPriceOriginal);
                cmd.Parameters.AddWithValue("@Sell_Coin_Original_Price", sellInfo.OriginalSellPrice);
                cmd.Parameters.AddWithValue("@Sell_Trade_Type", sellInfo.TradeType);

                cmd.Parameters.AddWithValue("@Memo", memo);
                cmd.ExecuteNonQuery();

                myLogger.Warn($"USDT MARGIN = {usdtMargin}");
                myLogger.Warn($"BUY[{buyInfo.MostMarket.ToString()} ,Price:{buyInfo.AvgPriceOriginal} ,Qty:{buyInfo.TotalFilledQty} ,Type:{buyInfo.TradeType}]");
                myLogger.Warn($"SELL[{sellInfo.MostMarket.ToString()} ,Price:{sellInfo.AvgPriceOriginal} ,Qty:{sellInfo.TotalFilledQty} ,Type:{sellInfo.TradeType}]");
            }
            catch (Exception error)
            {
                myLogger.Debug("error while DBController\n" + error.ToString());
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public void StoreExceptionInfo(COIN_TYPE coinType, string memo)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "Log_Exception_Insert");

            try
            {
                myLogger.Error("!!!!!!");
                cmd.Connection.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Coin_Type", coinType.ToString());
                cmd.Parameters.AddWithValue("@Memo", memo.Replace(Environment.NewLine, " ").Replace("\r\n", " ").Replace("\n", " "));

                cmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                myLogger.Debug("error while DBController\n" + error.ToString());
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        public void StoreTradingTime(COIN_TYPE coinType, bool isStart)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Coin_Trading_Update");

            try
            {
                cmd.Connection.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Coin_Name", coinType.ToString());
                cmd.Parameters.AddWithValue("@Trading_Time", isStart ? TimeManager.SyncronizedTime : new DateTime(1970, 1, 1));
                cmd.Parameters.AddWithValue("@IsTrading", isStart);

                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        public void StoreHandlePendingTime(COIN_TYPE coinType, bool isStart)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Coin_HandlePending_Update");

            try
            {
                cmd.Connection.Open();
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@Coin_Name", coinType.ToString());
                cmd.Parameters.AddWithValue("@HandlePending_Time", isStart ? TimeManager.SyncronizedTime : new DateTime(1970, 1, 1));
                cmd.Parameters.AddWithValue("@HandlePending", isStart);

                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        public void StoreLastTradeTime(COIN_MARKET market, bool isStart)
        {
            SqlConnection conn = new SqlConnection(CONNECT_STRING);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MarginDev_Market_LastTradeTime_Update";
            try
            {
                conn.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Market_Name", market.ToString());
                cmd.Parameters.AddWithValue("@LastTradeTime", isStart ? TimeManager.SyncronizedTime : new DateTime(1970, 1, 1));
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                conn.Close();
            }
        }

        public void updateLastFuckTime(COIN_MARKET coinMarket)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Market_LastFuckTime_Update");

            try
            {
                cmd.Connection.Open();
                cmd.Parameters.Clear();
                Constants.LastFuckTime[(int)coinMarket] = TimeManager.UtcTimeMS();
                cmd.Parameters.AddWithValue("@Market_Name", coinMarket.ToString());
                cmd.Parameters.AddWithValue("@LastFuckTime", TimeManager.SyncronizedTime);

                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        public void StoreClientVersion(COIN_TYPE coinType)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Version_Update");

            try
            {
                cmd.Connection.Open();
                cmd.Parameters.Clear();
                Assembly assem = Assembly.GetEntryAssembly();
                AssemblyName assemName = assem.GetName();
                Version ver = assemName.Version;
                myLogger.Warn($"Current Client version is {ver}");

                cmd.Parameters.AddWithValue("@Coin_Name", coinType.ToString());
                cmd.Parameters.AddWithValue("@Version", ver.ToString());

                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        public bool LoadMarketTradingData()
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Market_Trading_Select");
            DataTable table = new DataTable();

            try
            {
                cmd.Connection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(table);

                //for (int i = 0; i < table.Rows.Count; i++)
                for (int i = 0; i < 4; i++)
                {
                    Constants.LastOrderTime[i] = TimeManager.getMsTimeFromDateTime((DateTime)table.Rows[i]["LastTradeTime"]);
                    Constants.LastFuckTime[i] = TimeManager.getMsTimeFromDateTime((DateTime)table.Rows[i]["LastFuckTime"]);
                }
                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error(err.ToString());
                return false;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return true;
        }

        private Dictionary<string, string> LoadCoreOptionData()
        {
            Dictionary<string, string> optionDic = new Dictionary<string, string>();

            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_CoreOption_Advanced_Select");
            try
            {
                cmd.Connection.Open();

                DataSet ds = new DataSet();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds);

                DataTable coinCoreOptionTable = ds.Tables[0];
                DataTable marketCoreOptionTable = ds.Tables[1];
                DataTable marketCoinCoreOption = ds.Tables[2];
                DataTable generalCoinCoreOptionTable = ds.Tables[3];
                DataTable generalMarketCoreOptionTable = ds.Tables[4];

                optionDic.Add($"NUCLEAR", generalMarketCoreOptionTable.Rows[0]["NUCLEAR"].ToString());
                optionDic.Add("PendingTimeInSecond", generalMarketCoreOptionTable.Rows[0]["Waiting_Time_For_Open_Orders"].ToString());
                optionDic.Add("OrderbookPeriod", generalMarketCoreOptionTable.Rows[0]["OrderBook_Valid_Time"].ToString());
                optionDic.Add("OrderbookLimit", generalMarketCoreOptionTable.Rows[0]["OrderBook_Confirmation_Cycle"].ToString());

                //this.LoadCoinCoreOption(marketCoinCoreOption, optionDic);

                for (int i = 0; i < marketCoreOptionTable.Rows.Count; i++)
                {
                    COIN_MARKET currentMarket;
                    if (!Enum.TryParse<COIN_MARKET>(marketCoreOptionTable.Rows[i]["Market_Name"].ToString(), out currentMarket))
                    {
                        continue;
                    }

                    string marketName = currentMarket.ToString();

                    //optionDic.Add("Enabled", coinTable.Rows[(int)coinMode]["Toggle_OnOff"].ToString());
                    //optionDic.Add("PriceDiffThreshold", coinTable.Rows[(int)coinMode]["PriceDiffThreshold"].ToString());
                    //optionDic.Add("Bound", coinTable.Rows[(int)coinMode]["Quotation"].ToString());
                    //optionDic.Add("MarginThreshold", coinTable.Rows[(int)coinMode]["Min_Devouring"].ToString());

                    //optionDic.Add($"{marketName}_AutoControl", marketCoreOptionTable.Rows[i]["AutoControl"].ToString());
                    optionDic.Add($"{marketName}_Enable", marketCoreOptionTable.Rows[i]["Market_Enable"].ToString());
                    optionDic.Add($"{marketName}_Leverage", marketCoreOptionTable.Rows[i]["Leverage"].ToString());
                    optionDic.Add($"{marketName}_API_KEY", marketCoreOptionTable.Rows[i]["API_KEY"].ToString());
                    optionDic.Add($"{marketName}_SECRET_KEY", marketCoreOptionTable.Rows[i]["SECRET_KEY"].ToString());

                    optionDic.Add($"TradeFreq_{marketName}", marketCoreOptionTable.Rows[i]["Trade_Frequency_Sec"].ToString());
                    //optionDic.Add($"{marketName}_COIN_Enable", marketCoreOptionTable.Rows[i][$"{coinMode.ToString()}_Enable"].ToString());
                    optionDic.Add($"MakerFee_{marketName}", marketCoreOptionTable.Rows[i]["Maker_Fee"].ToString());
                    optionDic.Add($"TakerFee_{marketName}", marketCoreOptionTable.Rows[i]["Taker_Fee"].ToString());
                    optionDic.Add($"FeeAuto_{marketName}", marketCoreOptionTable.Rows[i]["Auto_Fee_Mode_OnOff"].ToString());
                    optionDic.Add($"{marketName}_Nuclear", marketCoreOptionTable.Rows[i]["Nuclear"].ToString());
                }

                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
                return null;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }

            return optionDic;
        }

        private void LoadGeneralCoreOption(DataTable dataTable, Dictionary<string, string> optValues)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                optValues.Add("PendingTimeInSecond", row.Field<int>("Waiting_Time_For_Open_Orders").ToString());
                optValues.Add("OrderbookPeriod", row.Field<int>("OrderBook_Valid_Time").ToString());
                optValues.Add("OrderbookLimit", row.Field<int>("OrderBook_Confirmation_Cycle").ToString());
            }
        }

        private void LoadMarketCoreOption(DataTable dataTable, string marketName, Dictionary<string, string> optValues)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                string tMarketName = row.Field<string>("Market_Name");

                if (tMarketName.Equals(marketName))
                {
                    optValues.Add("Market_Enable", row.Field<bool>("Market_Enable").ToString());
                    optValues.Add("Trade_Frequency_Sec", row.Field<int>("Trade_Frequency_Sec").ToString());
                    optValues.Add("Auto_Fee_Mode_OnOff", row.Field<bool>("Auto_Fee_Mode_OnOff").ToString());
                    optValues.Add("Taker_Fee", row.Field<double>("Taker_Fee").ToString());
                    optValues.Add("Maker_Fee", row.Field<double>("Maker_Fee").ToString());
                    optValues.Add("Leverage", row.Field<double>("Leverage").ToString());
                    optValues.Add("API_KEY", row.Field<string>("API_KEY"));
                    optValues.Add("SECRET_KEY", row.Field<string>("SECRET_KEY"));
                }
            }
        }

        private void LoadCoinCoreOption(DataTable dataTable, Dictionary<string, string> optValues)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                string tCoinName = row.Field<string>("Coin_Name");
                optValues.Add($"Coin_OnOff_{tCoinName}", row.Field<bool>("Coin_OnOff").ToString());
                optValues.Add($"PriceDiffThreshold_{tCoinName}", row.Field<double>("Conditions_For_Transaction_Occuerrence").ToString());
                optValues.Add($"Quotation_{tCoinName}", row.Field<double>("Fold_Order_Price").ToString());
                optValues.Add($"Min_Devouring_{tCoinName}", row.Field<double>("Minimum_Transaction_Amount").ToString());

                optValues.Add($"CER_{tCoinName}", "1");
                optValues.Add($"AREE_C_{tCoinName}", "100");
                optValues.Add($"AREE_P_{tCoinName}", "100");
                optValues.Add($"LAS_{tCoinName}", "1");
            }
        }

        private void LoadMarketCoinCoreOption(DataTable dataTable, string marketName, Dictionary<string, string> optValues)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                string tMarketName = row.Field<string>("Market_Name");
                string tCoinName = row.Field<string>("Coin_Name");

                // Assume - market Name and coinName combination is unique.
                if (tMarketName.Equals(marketName))
                {
                    optValues.Add($"Coin_Enabled_{tCoinName}", row.Field<bool>("Coin_Enable").ToString());
                    optValues.Add($"Order_Unit_{tCoinName}", row.Field<double>("Quotation_Unit").ToString());
                    optValues.Add($"Min_Trade_Value_{tCoinName}", row.Field<double>("Minimum_Order_Unit").ToString());
                }
            }
        }

        private Dictionary<string, string> legacyLoadCoreOptionData()
        {
            Dictionary<string, string> optionDic = new Dictionary<string, string>();
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_CoreOption_Select");

            try
            {
                cmd.Connection.Open();

                DataSet ds = new DataSet();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds);

                DataTable commonTable = ds.Tables[0];
                DataTable marketTable = ds.Tables[1];
                DataTable coinTable = ds.Tables[2];
                optionDic.Add($"NUCLEAR", commonTable.Rows[0]["NUCLEAR"].ToString());

                for (int i = 0; i < marketTable.Rows.Count; i++)
                {
                    COIN_MARKET currentMarket;
                    if (!Enum.TryParse<COIN_MARKET>(marketTable.Rows[i]["Market_Name"].ToString(), out currentMarket))
                    {
                        continue;
                    }

                    string marketName = currentMarket.ToString();

                    optionDic.Add($"{marketName}_AutoControl", marketTable.Rows[i]["AutoControl"].ToString());
                    optionDic.Add($"{marketName}_Enable", marketTable.Rows[i]["Enable"].ToString());
                    optionDic.Add($"{marketName}_Leverage", marketTable.Rows[i]["Leverage"].ToString());
                    optionDic.Add($"{marketName}_API_KEY", marketTable.Rows[i]["API_KEY"].ToString());
                    optionDic.Add($"{marketName}_SECRET_KEY", marketTable.Rows[i]["SECRET_KEY"].ToString());
                    //optionDic.Add($"TradeFreq_{marketName}", marketTable.Rows[i]["Trade_Frequency_Sec"].ToString());
                    //optionDic.Add($"{marketName}_COIN_Enable", marketCoreOptionTable.Rows[i][$"{coinMode.ToString()}_Enable"].ToString());
                    optionDic.Add($"MakerFee_{marketName}", marketTable.Rows[i]["Maker_Fee"].ToString());
                    optionDic.Add($"TakerFee_{marketName}", marketTable.Rows[i]["Taker_Fee"].ToString());
                }

                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
                return null;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }

            return optionDic;
        }

        public bool LoadCoinTradingData()
        { // 완료
            DataTable table = new DataTable();
            SqlConnection conn = new SqlConnection(CONNECT_STRING);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandText = "MarginDev_Coin_Trading_Select";

            try
            {
                conn.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(table);

                for (int i = 0; i < Constants.COIN_COUNT; i++)
                {
                    Constants.handlePendingTime[i] = TimeManager.getInstance().ConvertTimeToMs((DateTime)table.Rows[i]["HandlePending_Time"]);
                    Constants.tradingTime[i] = TimeManager.getInstance().ConvertTimeToMs((DateTime)table.Rows[i]["Trading_Time"]);
                }

                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error(err.ToString());
                return false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return true;
        }

        public DateTime SelectPLDailyLogDataLastDateTime()
        { // 완료
            SqlConnection conn = new SqlConnection(CONNECT_STRING);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MarginDev_PLDailyLog_Select_Recent";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Recent", 1);

            DateTime dt = TimeManager.SyncronizedTime;
            try
            {
                conn.Open();

                SqlDataReader dataReader = cmd.ExecuteReader();
                dataReader.Read();
                dt = Convert.ToDateTime(dataReader["Date_Time"]);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        public void updateNuclearState(bool value)
        { // 보류
            string conStr = CONNECT_STRING;
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MarginDev_NUCLEAR_Update";

            try
            {
                conn.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@NUCLEAR", value);

                cmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                myLogger.Debug("error while DBController\n" + error.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public void updateMarketEnabledData(COIN_MARKET coinMarket, bool enabled)
        { // 완료
            string conStr = CONNECT_STRING;
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "MarginDev_Market_Enable_Update";
            try
            {
                conn.Open();
                command.Parameters.Clear();

                command.Parameters.AddWithValue("@Market_Name", coinMarket.ToString());
                command.Parameters.AddWithValue("@Enable", enabled);

                command.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }
        }

        public void updateExceptionLog(string memo)
        { // 보류
            string conStr = CONNECT_STRING;
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Log_Exception_Insert";

            try
            {
                myLogger.Error("!!!!!!");
                conn.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Coin_Type", "SERVER");
                cmd.Parameters.AddWithValue("@Memo", memo.Replace(Environment.NewLine, " ").Replace("\r\n", " ").Replace("\n", " "));

                cmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                myLogger.Debug("error while DBController\n" + error.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public Dictionary<string, string> LoadCoreOptionsByMarket(string marketName)
        {
            Dictionary<string, string> optionDic = new Dictionary<string, string>();

            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_CoreOption_Advanced_Select");
            try
            {
                cmd.Connection.Open();

                DataSet ds = new DataSet();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds);

                DataTable coinCoreOptionTable = ds.Tables[0];
                DataTable marketCoreOptionTable = ds.Tables[1];
                DataTable marketCoinCoreOption = ds.Tables[2];
                DataTable generalCoinCoreOptionTable = ds.Tables[3];
                DataTable generalMarketCoreOptionTable = ds.Tables[4];

                this.LoadCoinCoreOption(coinCoreOptionTable, optionDic);
                this.LoadMarketCoinCoreOption(marketCoinCoreOption, marketName, optionDic);
                this.LoadMarketCoreOption(marketCoreOptionTable, marketName, optionDic);
                this.LoadGeneralCoreOption(generalMarketCoreOptionTable, optionDic);

                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
                return null;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }

            return optionDic;
        }

        public void RegisterMarketDatabase(IMarket market)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_CoreOption_Advanced_Select");
            try
            {
                cmd.Connection.Open();

                DataSet ds = new DataSet();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds);

                DataTable marketCoinCoreOption = ds.Tables[2];

                bool isExist = false;
                foreach (DataRow row in marketCoinCoreOption.Rows)
                {
                    string tMarketName = row.Field<string>("Market_Name");

                    if (tMarketName.Equals(market.GetMyMarketName().ToString()))
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist)
                {
                    this.InsertCoreRelationValues(market.GetMyMarketName().ToString());
                    this.InsertBalanceColumnValues(market.GetMyMarketName().ToString());
                    this.InsertMarketCoreValues(market.GetMyMarketName().ToString());
                }

                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }

        private bool InsertBalanceColumnValues(string marketName)
        {
            SqlCommand cmd = this.createConnectionAndCommander(
                CommandType.Text,
                "INSERT INTO MarginDev_Balance_Advanced(Market_Name, USDT, USDT_LOCKED, Date_Time) VALUES (@Market_Name, @USDT, @USDT_LOCKED, @Date_Time)");
            try
            {
                cmd.Connection.Open();

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Market_Name", marketName);
                cmd.Parameters.AddWithValue("@USDT", 0);
                cmd.Parameters.AddWithValue("@USDT_LOCKED", 0);
                cmd.Parameters.AddWithValue("@Date_Time", TimeManager.SyncronizedTime);
                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }

            return true;
        }

        private bool InsertCoreRelationValues(string marketName)
        {
            SqlCommand cmd = this.createConnectionAndCommander(CommandType.StoredProcedure, "MarginDev_Market_Coin_CoreOption_Relation_Insert");
            try
            {
                cmd.Connection.Open();

                // Temporary
                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Market_Name", marketName);
                    cmd.Parameters.AddWithValue("@Coin_Name", coinType.ToString());
                    cmd.Parameters.AddWithValue("@Coin_Enable", true);
                    cmd.Parameters.AddWithValue("@Quotation_Unit", 0);
                    cmd.Parameters.AddWithValue("@Minimum_Order_Unit", 0);
                    cmd.ExecuteNonQuery();
                }

                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }

            return true;
        }

        private bool InsertMarketCoreValues(string marketName)
        {
            SqlCommand cmd = this.createConnectionAndCommander(
                CommandType.Text,
                "INSERT INTO " +
                "MarginDev_Market_CoreOption_Advanced(" +
                "Market_Name, Market_Enable,Trade_Frequency_Sec,Auto_Fee_Mode_OnOff,Taker_Fee,Maker_Fee,Pause_In_FundingFee_Settlement,Premium_Adjustment,Memo,VIP,Leverage,API_KEY,SECRET_KEY,API_Date_Time,Nuclear)" +
                "VALUES (@Market_Name,@Market_Enable,@Trade_Frequency_Sec,@Auto_Fee_Mode_OnOff,@Taker_Fee,@Maker_Fee,@Pause_In_FundingFee_Settlement,@Premium_Adjustment,@Memo,@VIP,@Leverage,@API_KEY,@SECRET_KEY,@API_Date_Time,@Nuclear)");
            try
            {
                cmd.Connection.Open();

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Market_Name", marketName);
                cmd.Parameters.AddWithValue("@Market_Enable", true);
                cmd.Parameters.AddWithValue("@Trade_Frequency_Sec", 1);
                cmd.Parameters.AddWithValue("@Auto_Fee_Mode_OnOff", false);
                cmd.Parameters.AddWithValue("@Taker_Fee", 0);
                cmd.Parameters.AddWithValue("@Maker_Fee", 0);
                cmd.Parameters.AddWithValue("@Pause_In_FundingFee_Settlement", 0);
                cmd.Parameters.AddWithValue("@Premium_Adjustment", 0);
                cmd.Parameters.AddWithValue("@Memo", "Memo");
                cmd.Parameters.AddWithValue("@VIP", false);
                cmd.Parameters.AddWithValue("@Leverage", 0);
                cmd.Parameters.AddWithValue("@API_KEY", "TestKey");
                cmd.Parameters.AddWithValue("@SECRET_KEY", "TestKey");
                cmd.Parameters.AddWithValue("@API_Date_Time", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@Nuclear", false);
                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            catch (Exception err)
            {
                myLogger.Error("error!!!!" + err);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }

            return true;
        }

        private Position CreateDefaultPosition(COIN_MARKET coinMarket, COIN_TYPE coinType)
        {
            Position pos = new Position(coinMarket, coinType);

            pos.SetPrice(POSITION_SIDE.LONG, 0);
            pos.SetQuantity(POSITION_SIDE.LONG, 0);
            pos.SetPNL(POSITION_SIDE.LONG, 0);
            pos.SetLeverage(POSITION_SIDE.LONG, 0);

            pos.SetPrice(POSITION_SIDE.SHORT, 0);
            pos.SetQuantity(POSITION_SIDE.SHORT, 0);
            pos.SetPNL(POSITION_SIDE.SHORT, 0);
            pos.SetLeverage(POSITION_SIDE.SHORT, 0);

            pos.Total = 0;

            return pos;
        }

        private IDictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>> CreateDefaultPositionSet()
        {
            IDictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>> positions =
                new Dictionary<COIN_MARKET, IDictionary<COIN_TYPE, Position>>();

            foreach (COIN_MARKET marketName in Enum.GetValues(typeof(COIN_MARKET)))
            {
                positions.Add(marketName, new Dictionary<COIN_TYPE, Position>());

                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    positions[marketName].Add(coinType, this.CreateDefaultPosition(marketName, coinType));
                }
            }

            return positions;
        }
    }
}