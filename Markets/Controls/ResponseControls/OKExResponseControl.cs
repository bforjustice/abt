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

    public class OKExResponseControl : ResponseControlBase
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
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.OKEX, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["data"][0]["asks"][depth][0]),
                        Convert.ToDouble(res["data"][0]["asks"][depth][1]) * settings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["data"][0]["bids"][depth][0]),
                        Convert.ToDouble(res["data"][0]["bids"][depth][1]) * settings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.OKEX.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.OKEX.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.OKEX.ToString()}] {res.ToString()}");
                return OrderBook.Empty();
            }
        }

        public override Balance ParseBalance(Settings settings, APIResult result)
        {
            Balance newBalance = new Balance();
            JObject rawRes = JObject.Parse(result.Result);

            newBalance.Balance_USDT = Convert.ToDouble(rawRes.SelectToken("$.data..details[?(@.ccy == 'USDT')]..availEq").Value<string>());
            //newBalance.BalanceLocked_USDT = Convert.ToDouble(rawRes["data"][0]["frozenBal"]);
            //newBalance.CoinBalanceLocked_USDT.Add(coinType, Convert.ToDouble(rawRes["info"]["margin_frozen"])); //레버리지 곱해야하나?

            newBalance.Market = COIN_MARKET.OKEX;
            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.OKEX);
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
                foreach (JObject data in rawRes["data"])
                {
                    foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                    {
                        if (data["instId"].ToString().Equals($"{coinType.ToString()}-USDT-SWAP"))
                        {
                            Position pos = posList.Where((p) => p.CoinType.Equals(coinType)).First();

                            if (pos.Equals(null))
                            {
                                continue;
                            }

                            if (data["posSide"].ToString().Equals("short"))
                            {
                                pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble(data["avgPx"]));
                                pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(data["pos"]) * this.mySettings.GetOrderUnit(coinType));
                                pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(data["upl"]));
                                pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(data["lever"]));
                            }
                            else if (data["posSide"].ToString().Equals("long"))
                            {
                                //// ToDo
                                pos.SetPrice(POSITION_SIDE.LONG, Convert.ToDouble(data["avgPx"]));
                                pos.SetQuantity(POSITION_SIDE.LONG, Convert.ToDouble(data["pos"]) * this.mySettings.GetOrderUnit(coinType));
                                pos.SetPNL(POSITION_SIDE.LONG, Convert.ToDouble(data["upl"]));
                                pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(data["lever"]));
                            }

                            posList.Add(pos);
                        }
                    }
                }

                return posList;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.OKEX.ToString()}] getPosition exception!\n" + e);
                throw new InvalidOperationException("getPosition Exception!");
            }
        }

        public override OrderInfo ParseOrderInfo(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.OKEX.ToString()} OpenOrderInfo] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.OKEX, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.OKEX, result);

                    orderInfo.OrderId = res.SelectToken("$.data..ordId").Value<string>();

                    double totalQty = Convert.ToDouble(res.SelectToken("$.data..sz")) * settings.GetOrderUnit(orderInfo.CoinType);
                    orderInfo.FilledQty = Convert.ToDouble(res.SelectToken("$.data..accFillSz")) * settings.GetOrderUnit(orderInfo.CoinType);
                    orderInfo.RemainQty = totalQty - orderInfo.FilledQty;
                    orderInfo.EntryPrice = Convert.ToDouble(res.SelectToken("$.data..px").Value<string>());
                    orderInfo.AvgPrice = res.SelectToken("$.data..avgPx").HasValues ? Convert.ToDouble(res.SelectToken("$.data..avgPx")) : 0;
                    orderInfo.Side = res.SelectToken("$.data..side").Value<string>().Equals("buy") ? ORDER_SIDE.buy : ORDER_SIDE.sell;
                    orderInfo.Identifier = result.Identifier;

                    string status = res.SelectToken("$.data..state").Value<string>();

                    switch (status)
                    {
                        case "canceled":
                            orderInfo.PendingType = PENDING_TYPE.CANCEL_AND_REORDER;
                            break;
                        case "filled":
                            orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                            break;
                        case "partially_filled":
                        case "live":
                            orderInfo.PendingType = PENDING_TYPE.PARTIAL;
                            break;
                        default:
                            orderInfo.PendingType = PENDING_TYPE.UNKNOWN;
                            break;
                    }

                    return orderInfo;
                }

                return OrderInfo.Invaild(COIN_MARKET.OKEX, new Exception());

            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.OKEX}, OpenOrderInfo Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.OKEX, e);
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
                myLogger.Error($"[{COIN_MARKET.OKEX.ToString()} PlaceOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.OKEX, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.OKEX, result);

                if (res.HasValues)
                {
                    info.OrderId = res["data"][0]["ordId"].ToString();
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.OKEX, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.OKEX}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.OKEX, e);
            }
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.OKEX.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.OKEX, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.OKEX, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    info.OrderId = res["data"][0]["ordId"].ToString();
                    return info;
                }
                else
                {
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.OKEX, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.OKEX}, Cancel OrderParse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.OKEX, e);
            }

        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.OKEX;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$.data..instId");
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