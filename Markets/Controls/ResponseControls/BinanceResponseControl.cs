namespace Markets.Controls.ResponseControls
{
    using Configuration;
    using DataModels;
    using Markets.Converters;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BinanceResponseControl : ResponseControlBase
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

                orderBook.Market = COIN_MARKET.BINANCE;
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
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.BINANCE, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["asks"][depth][0]),
                        Convert.ToDouble(res["asks"][depth][1]),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["bids"][depth][0]),
                        Convert.ToDouble(res["bids"][depth][1]),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.BINANCE.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.BINANCE.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.BINANCE.ToString()}] {res.ToString()}");
                return OrderBook.Empty();
            }
        }

        public override Balance ParseBalance(Settings settings, APIResult result)
        {
            JObject res;
            try
            {
                res = JObject.Parse(result.Result);
            }
            catch (Exception e)
            {
                return Balance.Create(e);
            }

            Balance newBalance = new Balance();

            newBalance.Balance_USDT = Convert.ToDouble(res["availableBalance"]);
            foreach (var value in res["positions"])
            {
                foreach (COIN_TYPE coin in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (value["symbol"].ToString().Equals(coin + "USDT"))
                    {
                        newBalance.CoinBalanceLocked_USDT.Add(coin, Convert.ToDouble(value["openOrderInitialMargin"]));
                    }
                }
            }

            newBalance.Market = COIN_MARKET.BINANCE;

            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.BINANCE);
            JArray rawRes;
            try
            {
                rawRes = JArray.Parse(result.Result);
            }
            catch (Exception e)
            {
                myLogger.Error($"Position Parsing Error : {e.StackTrace} {e.ToString()}");
                return new List<Position>() { Position.Create(e) };
            }

            foreach (var value in rawRes)
            {
                string rawStr = value["symbol"].ToString().Replace("USDT", "");
                COIN_TYPE coin;
                if (!Enum.TryParse<COIN_TYPE>(rawStr, out coin))
                {
                    continue;
                }

                Position pos = posList.Where((p) => p.CoinType.Equals(coin)).First();

                if (pos.Equals(null))
                {
                    continue;
                }

                if (Convert.ToDouble(value["positionAmt"]) < 0)
                {
                    pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble(value["entryPrice"]));
                    pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(value["positionAmt"]) * -1);
                    pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(value["unrealizedProfit"]));
                    pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(value["leverage"]));
                }
                else if (Convert.ToDouble(value["positionAmt"]) > 0)
                {
                    pos.SetPrice(POSITION_SIDE.LONG, Convert.ToDouble(value["entryPrice"]));
                    pos.SetQuantity(POSITION_SIDE.LONG, Convert.ToDouble(value["positionAmt"]));
                    pos.SetPNL(POSITION_SIDE.LONG, Convert.ToDouble(value["unrealizedProfit"]));
                    pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(value["leverage"]));
                }

                posList.Add(pos);
            }

            return posList;
        }

        public override OrderInfo ParseOrderInfo(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BINANCE.ToString()} OpenOrderInfo] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BINANCE, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BINANCE, result);

                    double totalQty = Convert.ToDouble(res["origQty"]);
                    orderInfo.FilledQty = Convert.ToDouble(res["executedQty"]);
                    orderInfo.RemainQty = totalQty - orderInfo.FilledQty;
                    orderInfo.EntryPrice = Convert.ToDouble(res["price"].Value<string>());
                    orderInfo.AvgPrice = Convert.ToDouble(res["avgPrice"].Value<string>());
                    orderInfo.Side = res["side"].Value<string>().Equals("SELL") ? ORDER_SIDE.sell : ORDER_SIDE.buy;
                    orderInfo.OrderId = res["orderId"].ToString();

                    string status = res.SelectToken("$.status").Value<string>();

                    switch (status)
                    {
                        case "FILLED":
                            orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                            break;
                        case "EXPIRED":
                        case "CANCELED":
                            orderInfo.PendingType = PENDING_TYPE.CANCEL_AND_REORDER;
                            break;
                        case "PARTIALLY_FILLED":
                            orderInfo.PendingType = PENDING_TYPE.PARTIAL;
                            break;
                        case "NEW":
                            orderInfo.PendingType = PENDING_TYPE.NEW;
                            break;
                        default:
                            orderInfo.PendingType = PENDING_TYPE.UNKNOWN;
                            break;
                    }

                    return orderInfo;
                }

                return OrderInfo.Invaild(COIN_MARKET.BINANCE, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BINANCE}, OpenOrderInfo Error : {e.StackTrace} {e.ToString()}");
                myLogger.Error($"{result.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BINANCE, e);
            }
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            JObject obj = JObject.Parse(result.Result);
            return obj["leverage"].ToString();
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BINANCE.ToString()} PlaceOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BINANCE, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);
                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BINANCE, result);

                if (res.HasValues)
                {
                    info.OrderId = res["orderId"].ToString();
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.BINANCE, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BINANCE}, OpenOrderInfo Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BINANCE, e);
            }

        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BINANCE.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BINANCE, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);
                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BINANCE, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    if (res["status"] != null && res["status"].Value<string>().Equals("CANCELED"))
                    {
                        info.OrderId = res["orderId"].ToString();
                        return info;
                    }
                    else
                    {
                        myLogger.Error("Invaild Request");
                        myLogger.Error($"{result.ToString()}");
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.BINANCE, result.Exception);
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BINANCE}, Cancel Error : {e.StackTrace} {e.ToString()}");
                myLogger.Error($"{result.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BINANCE, e);
            }
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JArray res = JArray.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.BINANCE;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$..symbol");
                foreach (JToken symbol in symbols)
                {
                    string rawSym = symbol.Value<string>();

                    if (rawSym.Substring(rawSym.Length - "USDT".Length).Equals("USDT"))
                    {
                        ticker.SetSymbols(rawSym);
                        ticker.SetCoin(CoinSymbolConverter.ConvertSymbolToCoinName(ticker.Market, rawSym));
                    }
                }
            }

            return ticker;
        }
    }
}