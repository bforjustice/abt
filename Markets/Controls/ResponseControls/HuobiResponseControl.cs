namespace Markets.Controls.ResponseControls
{
    using Configuration;
    using DataModels;
    using DataModels.Exceptions;
    using Markets.Converters;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class HuobiResponseControl : ResponseControlBase
    {
        public override OrderBook ParseOrderBookByWebSocket(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override OrderBook ParseOrderBook(Settings settings, APIResult result)
        {
            JObject res;
            try
            {
                res = JObject.Parse(result.Result);
            }
            catch (Exception e)
            {
                return OrderBook.Create(e);
            }

            try
            {
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.HUOBI, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["tick"]["asks"][depth][0]),
                        Convert.ToDouble(res["tick"]["asks"][depth][1]) * this.mySettings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["tick"]["bids"][depth][0]),
                        Convert.ToDouble(res["tick"]["bids"][depth][1]) * this.mySettings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.HUOBI.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.HUOBI.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.HUOBI.ToString()}] {res.ToString()}");
                return OrderBook.Empty();
            }
        }

        public override Balance ParseBalance(Settings settings, APIResult result)
        {
            JObject rawRes;
            try
            {
                rawRes = JObject.Parse(result.Result);
            }
            catch (ApiCallException e)
            {
                return Balance.Create(e);
            }
            catch (Exception e)
            {
                return Balance.Create(e);
            }

            Balance newBalance = new Balance();

            newBalance.Balance_USDT = Convert.ToDouble(rawRes["data"][0]["withdraw_available"]);

            foreach (var value in rawRes["data"][0]["contract_detail"])
            {
                foreach (COIN_TYPE coin in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (value["symbol"].ToString().Equals(coin.ToString()))
                    {
                        newBalance.CoinBalanceLocked_USDT.Add(coin, Convert.ToDouble(value["margin_frozen"]));
                    }
                }
            }

            newBalance.Market = COIN_MARKET.HUOBI;
            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.HUOBI);
            JObject rawRes;
            try
            {
                rawRes = JObject.Parse(result.Result);
            }
            catch (ApiCallException e)
            {
                return new List<Position>() { Position.Create(e) };
            }
            catch (Exception e)
            {
                return new List<Position>() { Position.Create(e) };
            }

            foreach (JObject data in rawRes["data"])
            {
                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (data["contract_code"].ToString().Equals($"{coinType.ToString()}-USDT"))
                    {
                        Position pos = posList.Where((p) => p.CoinType.Equals(coinType)).First();

                        if (pos.Equals(null))
                        {
                            continue;
                        }

                        if (data["direction"].ToString().Equals("buy"))
                        {
                            pos.SetPrice(POSITION_SIDE.LONG, Convert.ToDouble(data["cost_open"]));
                            pos.SetQuantity(POSITION_SIDE.LONG, Convert.ToDouble(data["available"]) * this.mySettings.GetMinTradeValue(coinType));
                            pos.SetPNL(POSITION_SIDE.LONG, Convert.ToDouble(data["profit_unreal"]));
                            pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(data["lever_rate"]));
                        }
                        else if (data["direction"].ToString().Equals("sell"))
                        {
                            pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble(data["cost_open"]));
                            pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(data["available"]) * this.mySettings.GetMinTradeValue(coinType));
                            pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(data["profit_unreal"]));
                            pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(data["lever_rate"]));
                        }

                        posList.Add(pos);
                    }
                }
            }

            return posList;
        }

        public override OrderInfo ParseOrderInfo(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.HUOBI.ToString()} OpenOrderInfo] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.HUOBI, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    if (res["status"].ToString().Equals("ok"))
                    {
                        OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.HUOBI, result);

                        foreach (var value in res["data"])
                        {
                            double totalQty = Convert.ToDouble(value["volume"]) * settings.GetOrderUnit(orderInfo.CoinType);
                            orderInfo.FilledQty = Convert.ToDouble(value["trade_volume"]) * settings.GetOrderUnit(orderInfo.CoinType);
                            orderInfo.RemainQty = totalQty - orderInfo.FilledQty;
                            orderInfo.EntryPrice = value["price"].Value<double>();
                            orderInfo.AvgPrice = value["trade_avg_price"].HasValues ? value["trade_avg_price"].Value<double>() : 0;
                            orderInfo.OrderId = value["order_id_str"].ToString();
                            orderInfo.Side = value["direction"].Value<string>().Equals("sell") ? ORDER_SIDE.sell : ORDER_SIDE.buy;

                            int status = Convert.ToInt32(value["status"]);

                            switch (status)
                            {
                                case 1: // Ready to submit the orders 
                                case 2: // Ready to submit the orders
                                case 3: // Have sumbmitted the orders
                                    orderInfo.PendingType = PENDING_TYPE.NEW;
                                    break;
                                case 4: // Orders partially matched
                                case 5: // Orders cancelled with partially matched
                                    orderInfo.PendingType = PENDING_TYPE.PARTIAL;
                                    break;
                                case 6: // Orders fully matched
                                    orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                                    break;
                                case 7: // Orders cancelled
                                case 11: // Orders cancelling
                                    orderInfo.PendingType = PENDING_TYPE.CANCEL_AND_REORDER;
                                    break;
                                default:
                                    orderInfo.PendingType = PENDING_TYPE.UNKNOWN;
                                    break;
                            }

                            return orderInfo;
                        }
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.HUOBI, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.HUOBI}, OpenOrderInfo Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.HUOBI, e);
            }
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            IList<string> res = new List<string>();

            foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
            {
                JObject obj = JObject.Parse(result.Result);

                if (obj["status"].ToString().Equals("ok"))
                {
                    res.Add(obj["data"]["lever_rate"].ToString());
                }

                Thread.Sleep(2000);
            }

            return res.First();
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.HUOBI.ToString()} PlaceOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.HUOBI, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.HUOBI, result);

                if (res.HasValues)
                {
                    if (res["status"].ToString().Equals("ok"))
                    {
                        info.OrderId = res["data"]["order_id"].ToString();
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.HUOBI, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.HUOBI}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.HUOBI, e);
            }
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.HUOBI.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.HUOBI, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.HUOBI, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    if (res["status"].ToString().Equals("ok"))
                    {
                        info.OrderId = res["data"]["successes"].ToString();
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.HUOBI, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.HUOBI}, CancelOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.HUOBI, e);
            }
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.HUOBI;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$.data..contract_code");
                foreach (JToken symbol in symbols)
                {
                    string rawSym = symbol.Value<string>();
                    string convertedCoinName = CoinSymbolConverter.ConvertSymbolToCoinName(ticker.Market, rawSym);

                    if (convertedCoinName.Equals(string.Empty))
                    {
                        continue;
                    }

                    ticker.SetSymbols(rawSym);
                    ticker.SetCoin(convertedCoinName);
                }
            }

            return ticker;
        }
    }
}