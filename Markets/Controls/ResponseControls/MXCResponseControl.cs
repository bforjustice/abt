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

    public class MXCResponseControl : ResponseControlBase
    {
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
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.MXC, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["data"]["asks"][depth][0]),
                        Convert.ToDouble(res["data"]["asks"][depth][1]) * this.mySettings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["data"]["bids"][depth][0]),
                        Convert.ToDouble(res["data"]["bids"][depth][1]) * this.mySettings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.MXC.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.MXC.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.MXC.ToString()}] {res.ToString()}");
                return OrderBook.Empty();
            }
        }

        public override Balance ParseBalance(Settings settings, APIResult result)
        {
            Balance newBalance = new Balance();
            foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
            {
                JObject rawRes;
                try
                {
                    rawRes = JObject.Parse(result.Result);
                }
                catch (Exception e)
                {
                    return Balance.Create(e);
                }

                foreach (JObject item in rawRes["data"])
                {
                    if (item["currency"].Value<string>().Equals("USDT"))
                    {
                        newBalance.Balance_USDT = Convert.ToDouble(item["availableBalance"]);
                        newBalance.BalanceLocked_USDT = Convert.ToDouble(item["frozenBalance"]);
                    }
                    if (item["currency"].Value<string>().Equals(coinType.ToString()))
                    {
                        newBalance.CoinBalance_USDT.Add(coinType, Convert.ToDouble(item["availableBalance"]));
                        newBalance.CoinBalanceLocked_USDT.Add(coinType, Convert.ToDouble(item["frozenBalance"]));
                    }
                }
            }

            newBalance.Market = COIN_MARKET.MXC;

            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.MXC);
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

            foreach (var data in rawRes["data"])
            {
                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (data["symbol"].Value<string>().Equals($"{coinType.ToString()}_USDT"))
                    {
                        Position pos = posList.Where((p) => p.CoinType.Equals(coinType)).First();

                        if (pos.Equals(null))
                        {
                            continue;
                        }

                        //// Long
                        if (data["positionType"].Value<int>().Equals(1))
                        {
                            pos.SetQuantity(POSITION_SIDE.LONG, Convert.ToDouble(data["holdVol"]) * this.mySettings.GetMinTradeValue(coinType));
                            pos.SetPrice(POSITION_SIDE.LONG, data["holdAvgPrice"].Value<double>());
                            pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(data["leverage"].Value<int>()));
                            pos.SetPNL(POSITION_SIDE.LONG, data["realised"].Value<double>());
                        }
                        else
                        {
                            pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(data["holdVol"]) * this.mySettings.GetMinTradeValue(coinType));
                            pos.SetPrice(POSITION_SIDE.LONG, data["holdAvgPrice"].Value<double>());
                            pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(data["leverage"].Value<int>()));
                            pos.SetPNL(POSITION_SIDE.SHORT, data["realised"].Value<double>());
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
                myLogger.Error($"[{COIN_MARKET.MXC.ToString()} OpenOrderInfo] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.MXC, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    OrderInfo orderInfo = new OrderInfo();

                    orderInfo.OrderId = res["data"]["orderId"].ToString();
                    orderInfo.Type = result.Method;
                    orderInfo.Market = COIN_MARKET.MXC;

                    //double totalQty = Convert.ToDouble(res["size"]) * settings.MinTradeValue;
                    //orderInfo.FilledQty = Convert.ToDouble(res["filled_qty"]) * settings.MinTradeValue;
                    //orderInfo.AvgPrice = res["data"]["dealAvgPrice"].HasValues ? Convert.ToDouble(res["data"]["dealAvgPrice"]) : 0.0;
                    //orderInfo.RemainQty = totalQty - orderInfo.FilledQty;

                    //if (totalQty == orderInfo.FilledQty)
                    //{
                    //    orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                    //}
                    //else if (orderInfo.FilledQty == 0)
                    //{
                    //    orderInfo.PendingType = PENDING_TYPE.FULL;
                    //}
                    //else
                    //{
                    //    orderInfo.PendingType = PENDING_TYPE.PARTIAL;
                    //}

                    return orderInfo;
                }

                return OrderInfo.Invaild(COIN_MARKET.MXC, new Exception());

            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.MXC}, OpenOrderInfo Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.MXC, e);
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
                myLogger.Error($"[{COIN_MARKET.MXC.ToString()} PlaceOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.MXC, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.MXC, result);

                if (res.HasValues)
                {
                    info.OrderId = res["data"].ToString();
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.MXC, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.MXC}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.MXC, e);
            }
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.MXC.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.MXC, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.MXC, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    if (res["success"].ToString().ToLower().Equals("true"))
                    {
                        info.OrderId = res["data"][0]["orderId"].ToString();
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.MXC, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.MXC}, Cancel Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.MXC, e);
            }
        }

        public override OrderBook ParseOrderBookByWebSocket(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.MXC;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$.data..symbol");
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