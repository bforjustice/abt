namespace Markets.Controls.ResponseControls
{
    using Configuration;
    using DataModels;
    using DataModels.Exceptions;
    using Markets.Converters;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;

    public class BitgetResponseControl : ResponseControlBase
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
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.BITGET, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["asks"][depth][0]),
                        Convert.ToDouble(res["asks"][depth][1]) * settings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["bids"][depth][0]),
                        Convert.ToDouble(res["bids"][depth][1]) * settings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.BITGET.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.BITGET.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.BITGET.ToString()}] {res.ToString()}");
                return OrderBook.Empty();
            }
        }

        public override Balance ParseBalance(Settings settings, APIResult result)
        {
            Balance newBalance = new Balance();
            foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
            {
                JArray rawRes;
                try
                {
                    rawRes = JArray.Parse(result.Result);
                }
                catch (Exception e)
                {
                    return Balance.Create(e);
                }

                foreach (JObject item in rawRes)
                {
                    if (item["symbol"].Value<string>().Equals(string.Concat("cmt_", coinType.ToString(), "usdt").ToLower()))
                    {
                        newBalance.CoinBalance_USDT.Add(coinType, Convert.ToDouble(item["total_avail_balance"]));
                        newBalance.CoinBalanceLocked_USDT.Add(coinType, Convert.ToDouble(item["margin_frozen"]));
                    }
                }
            }

            newBalance.Market = COIN_MARKET.BITGET;

            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.BITGET);
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

            foreach (var data in rawRes)
            {
                var holding = data["holding"];
                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (holding[0]["symbol"].ToString().Equals($"cmt_{coinType.ToString().ToLower()}usdt"))
                    {
                        Position pos = new Position(COIN_MARKET.BITGET, (COIN_TYPE)Enum.Parse(typeof(COIN_TYPE), coinType.ToString()));
                        pos.SetQuantity(POSITION_SIDE.LONG, Convert.ToDouble(holding[0]["position"]) * this.mySettings.GetMinTradeValue(coinType));
                        if (pos.GetQuantity(POSITION_SIDE.LONG) == 0)
                        {
                            pos.SetPrice(POSITION_SIDE.LONG, 0);
                        }
                        else
                        {
                            pos.SetPrice(POSITION_SIDE.LONG, Convert.ToDouble(holding[0]["avg_cost"]));
                        }
                        pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(holding[0]["leverage"]));

                        pos.SetPNL(POSITION_SIDE.LONG, Convert.ToDouble(holding[0]["unrealized_pnl"]));

                        pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(holding[1]["position"]) * this.mySettings.GetMinTradeValue(coinType));
                        if (pos.GetQuantity(POSITION_SIDE.SHORT) == 0)
                        {
                            pos.SetPrice(POSITION_SIDE.SHORT, 0);
                        }
                        else
                        {
                            pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble(holding[1]["avg_cost"]));
                        }
                        pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(holding[0]["leverage"]));
                        pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(holding[1]["unrealized_pnl"]));

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
                myLogger.Error($"[{COIN_MARKET.BITGET.ToString()} OpenOrderInfo] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITGET, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BITGET, result);

                    double totalQty = Convert.ToDouble(res["size"]) * settings.GetMinTradeValue(orderInfo.CoinType);
                    orderInfo.FilledQty = Convert.ToDouble(res["filled_qty"]) * settings.GetMinTradeValue(orderInfo.CoinType);
                    orderInfo.AvgPrice = Convert.ToDouble(res["price_avg"]);
                    orderInfo.RemainQty = totalQty - orderInfo.FilledQty;
                    orderInfo.OrderId = res["order_id"].ToString();

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

                return OrderInfo.Invaild(COIN_MARKET.BITGET, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BITGET}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITGET, e);
            }
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);
            return res["short_leverage"].Value<string>();
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BITGET.ToString()} PlaceOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITGET, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BITGET, result);

                if (res.HasValues)
                {
                    info.OrderId = res["order_id"].ToString();
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.BITGET, new Exception());

            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BITGET}, OpenOrderInfo Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITGET, e);
            }
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BITGET.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITGET, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);
                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BITGET, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    if (res["result"].ToString().Equals("True"))
                    {
                        info.OrderId = res["order_id"].ToString();
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.BITGET, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BITGET}, Cancel Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITGET, e);
            }
        }

        public override OrderBook ParseOrderBookByWebSocket(Settings settings, APIResult result)
        {
            throw new NotImplementedException();
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JArray res = JArray.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.BITGET;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$..symbol");
                foreach (JToken symbol in symbols)
                {
                    string rawSym = symbol.Value<string>();

                    if (rawSym.Substring(rawSym.Length - "usdt".Length).Equals("usdt") &&
                        rawSym.Substring(0, "cmt_".Length).Equals("cmt_"))
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