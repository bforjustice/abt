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

    public class GateIOResponseControl : ResponseControlBase
    {
        public override OrderBook ParseOrderBookByWebSocket(Settings settings, APIResult result)
        {
            JObject res;
            OrderBook orderBook = new OrderBook();
            try
            {
                res = JObject.Parse(result.Result);
            }
            catch (Exception e)
            {
                return OrderBook.Create(e);
            }

            if (!res.GetValue("result").Equals(null) && res["result"].Equals("null"))
            {
                return OrderBook.Empty();
            }

            if (!res.GetValue("data").Equals(null))
            {
                for (int depth = 0; depth < 5; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["data"]["a"][depth][0]),
                        Convert.ToDouble(res["data"]["a"][depth][1]),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["data"]["b"][depth][0]),
                        Convert.ToDouble(res["data"]["b"][depth][1]),
                        depth);
                }

                orderBook.Market = COIN_MARKET.GATEIO;
                orderBook.ElspTime = result.ElspTime;
                return orderBook;
            }

            return OrderBook.Empty();
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
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.GATEIO, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["asks"][depth]["p"]),
                        Convert.ToDouble(res["asks"][depth]["s"]) * this.mySettings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["bids"][depth]["p"]),
                        Convert.ToDouble(res["bids"][depth]["s"]) * this.mySettings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.GATEIO.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.GATEIO.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.GATEIO.ToString()}] {res.ToString()}");
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

            newBalance.Balance_USDT = Convert.ToDouble(rawRes["available"]);
            newBalance.Market = COIN_MARKET.GATEIO;
            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.GATEIO);
            JArray rawRes;
            try
            {
                rawRes = JArray.Parse(result.Result);
            }
            catch (ApiCallException e)
            {
                return new List<Position>() { Position.Create(e) };
            }
            catch (Exception e)
            {
                return new List<Position>() { Position.Create(e) };
            }

            foreach (JObject data in rawRes)
            {
                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (data["contract"].ToString().Equals($"{coinType.ToString()}_USDT"))
                    {
                        Position pos = posList.Where((p) => p.CoinType.Equals(coinType)).First();

                        if (pos.Equals(null))
                        {
                            continue;
                        }

                        if (Convert.ToDouble(data["size"]) > 0)
                        {
                            pos.SetPrice(POSITION_SIDE.LONG, Convert.ToDouble(data["entry_price"]));
                            pos.SetQuantity(POSITION_SIDE.LONG,
                                (Convert.ToDouble(data["size"]) * this.mySettings.GetOrderUnit(coinType)));
                            pos.SetPNL(POSITION_SIDE.LONG, Convert.ToDouble(data["unrealised_pnl"]));
                            pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(data["leverage"]));
                        }
                        else if (Convert.ToDouble(data["size"]) < 0)
                        {
                            pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble(data["entry_price"]));
                            pos.SetQuantity(POSITION_SIDE.SHORT,
                                (Convert.ToDouble(data["size"]) * this.mySettings.GetOrderUnit(coinType)));
                            pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(data["unrealised_pnl"]));
                            pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(data["leverage"]));
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
                myLogger.Error($"[{COIN_MARKET.GATEIO.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.GATEIO, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.GATEIO, result);

                    double totalQty = Convert.ToDouble(res["size"]) * this.mySettings.GetOrderUnit(orderInfo.CoinType);
                    orderInfo.RemainQty = Convert.ToDouble(res["left"]) * this.mySettings.GetOrderUnit(orderInfo.CoinType);
                    orderInfo.FilledQty = totalQty - orderInfo.RemainQty;
                    orderInfo.EntryPrice = Convert.ToDouble(res["price"].ToString());
                    orderInfo.AvgPrice = Convert.ToDouble(res["fill_price"].ToString());
                    orderInfo.Market = COIN_MARKET.GATEIO;
                    orderInfo.Side = totalQty < 0 ? ORDER_SIDE.sell : ORDER_SIDE.buy;
                    orderInfo.Identifier = result.Identifier;

                    string status = res.SelectToken("$.status").Value<string>();

                    switch (status)
                    {
                        case "finished":
                            {
                                string subState= res.SelectToken("$.finish_as").Value<string>();
                                if (subState.Equals("filled"))
                                {
                                    orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                                }
                                else if (subState.Equals("cancelled"))
                                {
                                    orderInfo.PendingType = PENDING_TYPE.CANCEL_AND_REORDER;
                                }
                                else
                                {
                                    orderInfo.PendingType = PENDING_TYPE.UNKNOWN;
                                }
                            }
                            break;
                        case "open":
                            if (totalQty == orderInfo.FilledQty)
                            {
                                orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                            }
                            else if (orderInfo.FilledQty == 0)
                            {
                                orderInfo.PendingType = PENDING_TYPE.FULL;
                            }
                            else
                            {
                                orderInfo.PendingType = PENDING_TYPE.PARTIAL;
                            }

                            break;
                        default:
                            orderInfo.PendingType = PENDING_TYPE.UNKNOWN;
                            break;
                    }

                    return orderInfo;
                }

                return OrderInfo.Invaild(COIN_MARKET.GATEIO, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.GATEIO}, OpenOrderInfo Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.GATEIO, e);
            }
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);
            return res["leverage"].Value<string>();
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.GATEIO.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.GATEIO, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.GATEIO, result);

                if (res.HasValues)
                {
                    info.OrderId = res["id"].ToString();
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.GATEIO, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.GATEIO}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.GATEIO, e);
            }
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.GATEIO.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.GATEIO, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.GATEIO, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    info.OrderId = res["id"].ToString();
                    info.PendingType = res["finish_as"].ToString().Equals("cancelled") ? PENDING_TYPE.CANCELED : PENDING_TYPE.FULL;
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.GATEIO, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.GATEIO}, CancelOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.GATEIO, e);
            }
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JArray res = JArray.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.GATEIO;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$..contract");
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