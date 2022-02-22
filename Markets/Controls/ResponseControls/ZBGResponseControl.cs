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

    public class ZBGResponseControl : ResponseControlBase
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

            if (res == null)
            {
                return OrderBook.Empty();
            }

            try
            {
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.ZBG, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["datas"]["asks"][depth][0]),
                        Convert.ToDouble(res["datas"]["asks"][depth][1]),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["datas"]["bids"][depth][0]),
                        Convert.ToDouble(res["datas"]["bids"][depth][1]),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.ZBG.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.ZBG.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.ZBG.ToString()}] {res.ToString()}");
                return OrderBook.Empty();
            }
        }

        public override Balance ParseBalance(Settings settings, APIResult result)
        {
            Balance newBalance = new Balance();
            JObject rawRes = JObject.Parse(result.Result);

            if (rawRes["resMsg"]["method"].Value<string>().Equals("fail"))
            {
                return Balance.Create(new ApiCallException());
            }

            newBalance.Balance_USDT = Convert.ToDouble(rawRes["data"][0]["totalEq"]);
            //newBalance.CoinBalanceLocked_USDT.Add(coinType, Convert.ToDouble(rawRes["info"]["margin_frozen"])); //레버리지 곱해야하나?

            newBalance.Market = COIN_MARKET.ZBG;
            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.ZBG);
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

            try
            {
                foreach (JObject data in rawRes["datas"])
                {
                    foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                    {
                        Position pos = posList.Where((p) => p.CoinType.Equals(coinType)).First();

                        if (pos.Equals(null))
                        {
                            continue;
                        }

                        if (data["posiQty"].Value<double>() < 0)
                        {
                            pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble(data["initMargin"]));
                            pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(data["posiQty"]) * this.mySettings.GetMinTradeValue(coinType));
                            pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(data["closeProfitLoss"]));
                            pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(data["extraMargin"]));
                        }
                        else
                        {
                            //// ToDo
                            pos.SetPrice(POSITION_SIDE.LONG, Convert.ToDouble(data["initMargin"]));
                            pos.SetQuantity(POSITION_SIDE.LONG, Convert.ToDouble(data["posiQty"]) * this.mySettings.GetMinTradeValue(coinType));
                            pos.SetPNL(POSITION_SIDE.LONG, Convert.ToDouble(data["extraMargin"]));
                            pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(data["extraMargin"]));
                        }

                        posList.Add(pos);
                    }
                }

                return posList;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.ZBG.ToString()}] getPosition exception!\n" + e);
                throw new InvalidOperationException("getPosition Exception!");
            }
        }

        public override OrderInfo ParseOrderInfo(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.ZBG.ToString()} OpenOrderInfo]OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.ZBG, result.Exception);
            }

            try
            {

                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    //Todo
                    OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.ZBG, result);

                    // Identifier is Symbols

                    double totalQty = Convert.ToDouble(res["datas"][0]["orderQty"].ToString()) * this.mySettings.GetMinTradeValue(orderInfo.CoinType);
                    orderInfo.FilledQty = Convert.ToDouble(res["datas"][0]["matchQty"].ToString()) * this.mySettings.GetMinTradeValue(orderInfo.CoinType);
                    orderInfo.RemainQty = totalQty - orderInfo.FilledQty;
                    orderInfo.AvgPrice = Convert.ToDouble(res["datas"][0]["avgPrice"].ToString());
                    orderInfo.Market = COIN_MARKET.ZBG;
                    orderInfo.OrderId = res["datas"][0]["orderId"].ToString();

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

                    return orderInfo;
                }

                return OrderInfo.Invaild(COIN_MARKET.ZBG, new Exception());

            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.ZBG}, OpenOrderInfo Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.ZBG, e);
            }
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            JObject obj = JObject.Parse(result.Result);

            return (((int)Convert.ToDouble(obj["long_leverage"])).ToString());
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.ZBG.ToString()} CancelOrder] PlaceOrder Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.ZBG, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.ZBG, result);

                if (res.HasValues)
                {
                    info.OrderId = res["datas"].ToString();
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.ZBG, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.ZBG}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.ZBG, e);
            }
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.ZBG.ToString()} CancelOrder]OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.ZBG, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);
                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.ZBG, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    if (res["resMsg"]["code"].ToString().Equals("1"))
                    {
                        info.OrderId = res["datas"][0]["contractId"].ToString();
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.ZBG, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.ZBG}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.ZBG, e);
            }
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.ZBG;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$.datas..sb");
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