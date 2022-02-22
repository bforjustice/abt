namespace Markets.Controls.ResponseControls
{
    using Configuration;
    using DataModels;
    using Markets.Converters;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BitZResponseControl : ResponseControlBase
    {
        public override void PublishJob(APIResult result)
        {
            switch (result.Method)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    OrderBook book = result.SOURCE.Equals(DATA_SOURCE.REST) ?
                       this.ParseOrderBook(this.mySettings, result) :
                       this.ParseOrderBookByWebSocket(this.mySettings, result);
                    this.Notify(book);
                    break;

                case REQUEST_TYPE.GET_BALANCE:
                    this.Notify(this.ParseBalance(this.mySettings, result));
                    this.Notify(this.ParsePosition(this.mySettings, result));
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    this.Notify(this.ParseOrderInfo(this.mySettings, result));
                    break;

                case REQUEST_TYPE.SET_LEVERAGE:
                    this.ParseLeverage(this.mySettings, result);
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    this.Notify(this.ParsePlaceOrder(this.mySettings, result));
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    this.Notify(this.ParseCancelOrder(this.mySettings, result));
                    break;

                case REQUEST_TYPE.GET_POSITION:
                    //this.ParsePosition(this.mySettings, result);
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    this.Notify(this.ParseTickers(this.mySettings, result));
                    break;
            }

            result.DoneEvent.Set();
        }

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
                OrderBook orderBook = this.GenerateOrderBookWithParam(COIN_MARKET.BITZ, result, this.mySettings);

                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    orderBook.setDepth(ORDERBOOK_SIDE.ASK,
                        Convert.ToDouble(res["data"]["asks"][depth]["price"]),
                        Convert.ToDouble(res["data"]["asks"][depth]["amount"]) * this.mySettings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                    orderBook.setDepth(ORDERBOOK_SIDE.BID,
                        Convert.ToDouble(res["data"]["bids"][depth]["price"]),
                        Convert.ToDouble(res["data"]["bids"][depth]["amount"]) * this.mySettings.GetMinTradeValue(orderBook.CoinType),
                        depth);
                }

                return orderBook;
            }
            catch (Exception e)
            {
                myLogger.Error($"[{COIN_MARKET.BITZ.ToString()}] setOrderbook exception!");
                myLogger.Error($"[{COIN_MARKET.BITZ.ToString()}] StackTrace {e.StackTrace}");
                myLogger.Error($"[{COIN_MARKET.BITZ.ToString()}] {res.ToString()}");
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

            newBalance.Balance_USDT = Convert.ToDouble(res["data"]["estimate_USD"]);
            foreach (var value in res["data"]["balances"])
            {
                foreach (COIN_TYPE coin in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    if (value["coin"].Value<string>().Equals(coin.ToString()))
                    {
                        newBalance.CoinBalanceLocked_USDT.Add(coin, Convert.ToDouble(value["balance"]));
                    }
                }
            }

            newBalance.Market = COIN_MARKET.BITZ;

            return newBalance;
        }

        public override IList<Position> ParsePosition(Settings settings, APIResult result)
        {
            IList<Position> posList = this.GetDefaultPosition(COIN_MARKET.BITZ);
            JObject rawRes;
            try
            {
                rawRes = JObject.Parse(result.Result);
            }
            catch (Exception e)
            {
                return new List<Position>() { Position.Create(e) };
            }

            foreach (var value in rawRes["data"]["balances"])
            {
                string rawStr = value["coin"].ToString();
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

                if (Convert.ToDouble(value["total"]) < 0)
                {
                    pos.SetPrice(POSITION_SIDE.SHORT, Convert.ToDouble(value["entryPrice"]));
                    pos.SetQuantity(POSITION_SIDE.SHORT, Convert.ToDouble(value["positionAmt"]) * -1);
                    pos.SetPNL(POSITION_SIDE.SHORT, Convert.ToDouble(value["unrealizedProfit"]));
                    pos.SetLeverage(POSITION_SIDE.SHORT, Convert.ToDouble(value["leverage"]));
                }
                else if (Convert.ToDouble(value["total"]) > 0)
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
                myLogger.Error($"[{COIN_MARKET.BITZ.ToString()} OpenOrderInfo] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITZ, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                if (res.HasValues)
                {
                    OrderInfo orderInfo = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BITZ, result);

                    orderInfo.OrderId = res["data"]["orderId"].ToString();

                    if (res["data"]["orderStatus"].ToString().Equals("0"))
                    {
                        orderInfo.PendingType = PENDING_TYPE.PARTIAL;
                    }
                    else if (res["data"]["orderStatus"].ToString().Equals("1"))
                    {
                        orderInfo.PendingType = PENDING_TYPE.COMPLETE;
                    }

                    orderInfo.OrderId = res["data"]["orderId"].ToString();

                    return orderInfo;
                }

                return OrderInfo.Invaild(COIN_MARKET.BITZ, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BITZ}, OpenOrderInfo Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITZ, e);
            }
        }

        public override string ParseLeverage(Settings settings, APIResult result)
        {
            return string.Empty;
        }

        public override OrderInfo ParsePlaceOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BITZ.ToString()} PlaceOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITZ, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BITZ, result);

                if (res.HasValues)
                {
                    info.OrderId = res["data"]["orderId"].ToString();
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.BITZ, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BITZ}, PlaceOrder Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITZ, e);
            }
        }

        public override OrderInfo ParseCancelOrder(Settings settings, APIResult result)
        {
            if (this.CheckResultValidity(result))
            {
                myLogger.Error($"[{COIN_MARKET.BITZ.ToString()} CancelOrder] OrderInfo Invalid Parsing Error : {result.Exception.StackTrace} {result.Exception.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITZ, result.Exception);
            }

            try
            {
                JObject res = JObject.Parse(result.Result);

                OrderInfo info = this.ConvertAPIResultToOrderInfo(COIN_MARKET.BITZ, result);
                info.PendingType = PENDING_TYPE.CANCELED;

                if (res.HasValues)
                {
                    info.OrderId = res["data"]["orderId"].ToString();
                    return info;
                }

                return OrderInfo.Invaild(COIN_MARKET.BITZ, new Exception());
            }
            catch (Exception e)
            {
                myLogger.Error($"${COIN_MARKET.BITZ}, Cancel Parse Error : {e.StackTrace} {e.ToString()}");
                return OrderInfo.Invaild(COIN_MARKET.BITZ, e);
            }
        }

        public override Tickers ParseTickers(Settings settings, APIResult result)
        {
            JObject res = JObject.Parse(result.Result);

            Tickers ticker = new Tickers();
            ticker.Identifier = result.Identifier;
            ticker.Market = COIN_MARKET.BITZ;

            if (res.HasValues)
            {
                IEnumerable<JToken> symbols = res.SelectTokens("$.data..contractId");
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