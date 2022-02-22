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

    public class FTXResponseControl : ResponseControlBase
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
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.FTX, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["result"]["asks"][depth][0]),
                        Convert.ToDouble(res["result"]["asks"][depth][1]),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["result"]["bids"][depth][0]),
                        Convert.ToDouble(res["result"]["bids"][depth][1]),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.FTX.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.FTX.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.FTX.ToString()}] {res.ToString()}");
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

            foreach (var bal in rawRes["result"]["main"])
            {
                if (bal["coin"].Value<string>().Equals("USD"))
                {
                    newBalance.Balance_USDT = Convert.ToDouble(bal["availableWithoutBorrow"]);
                }
                else
                {
                    foreach (COIN_TYPE coin in Enum.GetValues(typeof(COIN_TYPE)))
                    {
                        if (bal["coin"].ToString().Equals(coin.ToString()))
                        {
                            newBalance.CoinBalanceLocked_USDT.Add(coin, Convert.ToDouble(bal["availableWithoutBorrow"]));
                        }
                    }
                }
            }

            newBalance.Market = COIN_MARKET.FTX;
            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.FTX);
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

            foreach (JObject data in rawRes["result"])
            {
                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (data["future"].ToString().Equals($"{coinType.ToString()}-PERP"))
                    {
                        Position pos = posList.Where((p) => p.CoinType.Equals(coinType)).First();

                        if (pos.Equals(null))
                        {
                            continue;
                        }

                        if (data["side"].ToString().Equals("buy") && !Convert.ToDouble(data["cost"]).Equals(0))
                        {
                            pos.SetPrice(POSITION_SIDE.LONG, Convert.ToDouble((data["entryPrice"] == null) ? 0 : data["entryPrice"]));
                            pos.SetQuantity(POSITION_SIDE.LONG, Convert.ToDouble(data["openSize"]));
                            pos.SetPNL(POSITION_SIDE.LONG, Convert.ToDouble(data["realizedPnl"]));
                            pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(data["openSize"]) / Convert.ToDouble(data["size"]));
                        }
                        else if (data["side"].ToString().Equals("sell") && !Convert.ToDouble(data["cost"]).Equals(0))
                        {
                            pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble((data["entryPrice"] == null) ? 0 : data["entryPrice"]));
                            pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(data["openSize"]));
                            pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(data["realizedPnl"]));
                            pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(data["openSize"]) / Convert.ToDouble(data["size"]));
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
                myLogger.Error($"[{COIN_MARKET.FTX.ToString()} OpenOrderInfo] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.FTX, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    if (res["success"].Value<bool>())
                    {
                        OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.FTX, result);

                        orderInfo.OrderId = res.SelectToken("$.result..id").Value<string>();

                        // Todo
                        orderInfo.FilledQty = res.SelectToken("$.result..filledSize").Value<double>();
                        orderInfo.RemainQty = res.SelectToken("$.result..remainingSize").Value<double>();
                        orderInfo.EntryPrice = res.SelectToken("$.result..price").Value<double>();
                        orderInfo.AvgPrice = res.SelectToken("$.result..avgFillPrice").HasValues ? 
                            res.SelectToken("$.result..avgFillPrice").Value<double>() : 0;
                        orderInfo.Side = res.SelectToken("$.result..side").Value<string>().Equals("sell") ? ORDER_SIDE.sell : ORDER_SIDE.buy;

                        double totalQty = res.SelectToken("$.result..size").Value<double>();
                        string status = res.SelectToken("$.result..status").Value<string>();

                        switch (status)
                        {
                            case "new":
                                orderInfo.PendingType = PENDING_TYPE.NEW;
                                break;
                            case "closed":
                            case "cancelled":
                            case "filled":
                                if (totalQty.Equals(orderInfo.FilledQty))
                                {
                                    orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                                }
                                else
                                {
                                    orderInfo.PendingType = PENDING_TYPE.CANCEL_AND_REORDER;
                                }
                                break;
                            default:
                                // Open
                                if (orderInfo.FilledQty.Equals(totalQty))
                                {
                                    orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                                }
                                else if (orderInfo.RemainQty.Equals(totalQty))
                                {
                                    orderInfo.PendingType = PENDING_TYPE.FULL;
                                }
                                else
                                {
                                    orderInfo.PendingType = PENDING_TYPE.PARTIAL;
                                }
                                break;
                        }

                        return orderInfo;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.FTX, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.FTX}, OpenOrderInfo Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.FTX, e);
            }
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);
            return res["success"].Value<string>();
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.FTX.ToString()} PlaceOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.FTX, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.FTX, result);

                if (res.HasValues)
                {
                    if (res["success"].ToString().Equals("True"))
                    {
                        info.OrderId = res["result"]["id"].ToString();
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.FTX, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"{COIN_MARKET.FTX}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.FTX, e);
            }

        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.FTX.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.FTX, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.FTX, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                myLogger.Info($"FTX Cancel Info :\n{info.ToString()}\n");

                if (res.HasValues)
                {
                    if (res["success"].Value<bool>().Equals(true))
                    {
                        return info;
                    }
                    else
                    {
                        if (res["result"].Value<string>().Equals("Order already queued for cancellation"))
                        {
                            return info;
                        }
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.FTX, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.FTX}, CancelOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.FTX, e);
            }
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.FTX;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$.result..name");
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