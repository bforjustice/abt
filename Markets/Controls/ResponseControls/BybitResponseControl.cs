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

    public class BybitResponseControl : ResponseControlBase
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

            if (res == null)
            {
                return OrderBook.Empty();
            }

            var asks = from b in res["result"].Children()
                       where b["side"].Value<string>().Equals("Buy")
                       select b;

            var bids = from s in res["result"].Children()
                       where s["side"].Value<string>().Equals("Sell")
                       select s;

            try
            {
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.BYBIT, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(asks.ToList()[depth]["price"]),
                        Convert.ToDouble(asks.ToList()[depth]["size"]),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(bids.ToList()[depth]["price"]),
                        Convert.ToDouble(bids.ToList()[depth]["size"]),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.BYBIT.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.BYBIT.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.BYBIT.ToString()}] {res.ToString()}");
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

            newBalance.Balance_USDT = Convert.ToDouble(rawRes["result"]["USDT"]["available_balance"]);
            newBalance.BalanceLocked_USDT = Convert.ToDouble(rawRes["result"]["USDT"]["order_margin"]);

            newBalance.Market = COIN_MARKET.BYBIT;
            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.BYBIT);
            JObject rawRes;
            try
            {
                rawRes = JObject.Parse(result.Result);
            }
            catch (Exception e)
            {
                return new List<Position>() { Position.Create(e) };
            }

            var arr = rawRes["result"];
            foreach (var value in arr)
            {
                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (value["data"]["symbol"].ToString().Equals($"{coinType.ToString()}USDT"))
                    {
                        Position pos = posList.Where((p) => p.CoinType.Equals(coinType)).First();

                        if (pos.Equals(null))
                        {
                            continue;
                        }

                        if (value["data"]["side"].ToString().Equals("Sell"))
                        {
                            pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble(value["data"]["entry_price"]));
                            pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(value["data"]["size"]));
                            pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(value["data"]["unrealised_pnl"]));
                            pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(value["data"]["leverage"]));
                        }
                        else if (value["data"]["side"].ToString().Equals("Buy"))
                        {
                            pos.SetPrice(POSITION_SIDE.LONG, Convert.ToDouble(value["data"]["entry_price"]));
                            pos.SetQuantity(POSITION_SIDE.LONG, Convert.ToDouble(value["data"]["size"]));
                            pos.SetPNL(POSITION_SIDE.LONG, Convert.ToDouble(value["data"]["unrealised_pnl"]));
                            pos.SetLeverage(POSITION_SIDE.LONG, Convert.ToDouble(value["data"]["leverage"]));
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
                myLogger.Error($"[{COIN_MARKET.BYBIT.ToString()} OpenOrderInfo] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BYBIT, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    if (res["ret_msg"].ToString().Equals("OK"))
                    {
                        if (res["result"].HasValues)
                        {
                            OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BYBIT, result);

                            double totalQty = Convert.ToDouble(res["result"]["qty"]);
                            double filledQty = Convert.ToDouble(res["result"]["cum_exec_qty"]);
                            double remainQty = totalQty - filledQty;

                            orderInfo.FilledQty = filledQty;
                            orderInfo.RemainQty = remainQty;
                            if (totalQty == filledQty)
                            {
                                orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                            }
                            else if (filledQty == 0)
                            {
                                orderInfo.PendingType = PENDING_TYPE.FULL;
                            }
                            else
                            {
                                orderInfo.PendingType = PENDING_TYPE.PARTIAL;
                            }

                            orderInfo.AvgPrice = filledQty == 0 ? 0 : Convert.ToDouble(res["result"]["cum_exec_value"]) / filledQty;
                            return orderInfo;
                        }
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.BYBIT, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BYBIT}, OpenOrderInfo Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BYBIT, e);
            }
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);
            return res["ret_msg"].Value<string>();
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BYBIT.ToString()} PlaceOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BYBIT, result.Exception);
            }
            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = new OrderInfo();
                info.Market = COIN_MARKET.BYBIT;
                info.Identifier = result.Identifier;
                info.OrderbookId = result.Tid;
                info.Type = result.Method;

                if (res.HasValues)
                {
                    if (res["ret_msg"].ToString().Equals("OK"))
                    {
                        info.OrderId = res["result"]["order_id"].ToString();
                        info.Identifier = result.Identifier;
                        info.OrderbookId = result.Tid;
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.BYBIT, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BYBIT}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BYBIT, e);
            }
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BYBIT.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BYBIT, result.Exception);
            }
            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BYBIT, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    if (res["ret_msg"].ToString().Equals("OK"))
                    {
                        info.OrderId = res["result"]["order_id"].ToString();
                        return info;
                    }
                }

                return OrderInfo.Invaild(COIN_MARKET.BYBIT, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BYBIT}, CancelOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BYBIT, e);
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
            ticker.Market = COIN_MARKET.BYBIT;

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