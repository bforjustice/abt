namespace CalculationEngine.LegacyStrategies
{
    using Common;
    using Configuration;
    using DataModels;
    using LogTrace.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Traders.Interfaces;

    public class CheckValidity
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CalcLogger");

        public CheckValidity()
        {
        }

        public double getPriceDiffThreshold(Data orderData)
        {
            // Temporary
            return (orderData.BuyMarketOrderUnit + orderData.SellMarketOrderUnit) * orderData.PriceDiffThreshold;
        }

        public bool checkMargin(OrderBooks orderBooks, ITrader trader, out Data result)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            IList<OrderBook> buyMarkets = orderBooks.BuyOrderbooks;
            IList<OrderBook> sellMarkets = orderBooks.SellOrderbooks;
            double maxPriceDiff = double.MinValue;
            double maxMargin = double.MinValue;
            double maxTradable = double.MinValue;

            Data PriceDiffData = new Data();
            Data MaxBenefitData = new Data();

            foreach (OrderBook buyMarket in buyMarkets)
            {
                foreach (OrderBook sellMarket in sellMarkets)
                {
                    if (buyMarket.Market.Equals(sellMarket.Market)
                        || !(trader.GetMarketState(buyMarket.Market).Equals(MARKET_STATE.NORMAL) && 
                            trader.GetMarketState(sellMarket.Market).Equals(MARKET_STATE.NORMAL)))
                    {
                        continue;
                    }

                    //// assume Available Trade
                    // for check PriceDiff
                    double priceDiff = this.PriceDiffApplyFee(buyMarket, sellMarket);

                    if (priceDiff > maxPriceDiff)
                    {
                        maxPriceDiff = priceDiff;
                        maxTradable = this.Tradable(buyMarket, sellMarket, trader);

                        PriceDiffData = this.CreateOrderData(buyMarket, sellMarket);
                        PriceDiffData.PriceDiffApplyFee = priceDiff;
                        PriceDiffData.Quantity = this.Quantity(buyMarket, sellMarket, trader);
                        PriceDiffData.TotalMargin = this.TotalMargin(buyMarket, sellMarket, trader);
                    }
                    else if (priceDiff == maxPriceDiff)
                    {
                        if (this.Tradable(buyMarket, sellMarket, trader) > maxTradable)
                        {
                            maxPriceDiff = priceDiff;
                            maxTradable = this.Tradable(buyMarket, sellMarket, trader);

                            PriceDiffData = this.CreateOrderData(buyMarket, sellMarket);
                            PriceDiffData.PriceDiffApplyFee = priceDiff;
                            PriceDiffData.Quantity = this.Quantity(buyMarket, sellMarket, trader);
                            PriceDiffData.TotalMargin = this.TotalMargin(buyMarket, sellMarket, trader);
                        }
                    }

                    //for check Max Benefit
                    double margin = this.TotalMargin(buyMarket, sellMarket, trader);
                    if (margin > maxMargin)
                    {
                        maxMargin = margin;
                        MaxBenefitData = this.CreateOrderData(buyMarket, sellMarket);
                        MaxBenefitData.TotalMargin = maxMargin;
                        MaxBenefitData.Quantity = this.Quantity(buyMarket, sellMarket, trader);
                        MaxBenefitData.PriceDiffApplyFee = this.PriceDiffApplyFee(buyMarket, sellMarket);
                    }
                }
            }

            //myLogger.Info($"PD 수수료제외 가격차이 : {PriceDiffData.PriceDiffApplyFee}, 수량 : {PriceDiffData.Quantity}, 마진 : {PriceDiffData.TotalMargin} BMarket : {PriceDiffData.BuyMarket.ToString()} SMarket :{PriceDiffData.SellMarket.ToString()}");
            //myLogger.Info($"MB 수수료제외 가격차이 : {MaxBenefitData.PriceDiffApplyFee}, 수량 : {MaxBenefitData.Quantity}, 마진 : {MaxBenefitData.TotalMargin} BMarket : {PriceDiffData.BuyMarket.ToString()} SMarket :{PriceDiffData.SellMarket.ToString()}");

            if (PriceDiffData.PriceDiffApplyFee > getPriceDiffThreshold(PriceDiffData) && PriceDiffData.PriceDiff > 0)
            {
                if (PriceDiffData.TotalMargin > PriceDiffData.MarginThreshold)
                {
                    if (PriceDiffData.Quantity >= PriceDiffData.BuyMarketMinTradeValue
                        && PriceDiffData.Quantity >= PriceDiffData.SellMarketMinTradeValue)
                    {
                        timer.Stop();
                        myLogger.Warn($"PriceDiff BuyMarket : {PriceDiffData.BuyMarket.ToString()} SellMarket : {PriceDiffData.SellMarket.ToString()} Alg : {orderBooks.OrderbookId}, Margin : {PriceDiffData.TotalMargin} ElspTime : {timer.ElapsedMilliseconds} ms");
                        PriceDiffData.BuyOrderPrice = PriceDiffData.DepthPrice(ORDERBOOK_SIDE.ASK)[0]
                            + PriceDiffData.BuyMarketBound * PriceDiffData.BuyMarketOrderUnit;
                        PriceDiffData.SellOrderPrice = PriceDiffData.DepthPrice(ORDERBOOK_SIDE.BID)[0]
                            - PriceDiffData.SellMarketBound * PriceDiffData.SellMarketOrderUnit;

                        trader.SetMarketState(PriceDiffData.SellMarket, MARKET_STATE.TRADING);
                        trader.SetMarketState(PriceDiffData.BuyMarket, MARKET_STATE.TRADING);

                        PriceDiffData.OrderbookId = orderBooks.OrderbookId;

                        result = PriceDiffData;
                        return true;
                    }
                }
            }

            if (MaxBenefitData.PriceDiffApplyFee > getPriceDiffThreshold(MaxBenefitData) && MaxBenefitData.PriceDiff > 0)
            {
                if (MaxBenefitData.TotalMargin > MaxBenefitData.MarginThreshold)
                {
                    if (MaxBenefitData.Quantity >= MaxBenefitData.BuyMarketMinTradeValue
                        && MaxBenefitData.Quantity >= MaxBenefitData.SellMarketMinTradeValue)
                    {
                        timer.Stop();
                        myLogger.Warn($"MaxBenefit BuyMarket : {PriceDiffData.BuyMarket.ToString()} SellMarket : {PriceDiffData.SellMarket.ToString()} Alg : {orderBooks.OrderbookId}, Margin : {MaxBenefitData.TotalMargin} ElspTime : {timer.ElapsedMilliseconds} ms");

                        MaxBenefitData.BuyOrderPrice = MaxBenefitData.DepthPrice(ORDERBOOK_SIDE.ASK)[0]
                            + MaxBenefitData.BuyMarketBound * MaxBenefitData.BuyMarketOrderUnit;
                        MaxBenefitData.SellOrderPrice = MaxBenefitData.DepthPrice(ORDERBOOK_SIDE.BID)[0]
                            - MaxBenefitData.SellMarketBound * MaxBenefitData.SellMarketOrderUnit;

                        trader.SetMarketState(MaxBenefitData.SellMarket, MARKET_STATE.TRADING);
                        trader.SetMarketState(MaxBenefitData.BuyMarket, MARKET_STATE.TRADING);

                        MaxBenefitData.OrderbookId = orderBooks.OrderbookId;
                        result = MaxBenefitData;
                        return true;
                    }
                }
            }

            timer.Stop();
            result = null;
            return false;
        }

        private double PriceDiffApplyFee(OrderBook buyMarket, OrderBook sellMarket)
        {
            //return (BidPrice[0] * (1 - Settings.TakerFee[(int)SellMarket]) - AskPrice[0] * (1 + Settings.TakerFee[(int)BuyMarket]));
            return (sellMarket.DepthPrice(ORDERBOOK_SIDE.BID)[0] * (1 - sellMarket.TakerFee)
                - buyMarket.DepthPrice(ORDERBOOK_SIDE.ASK)[0] * (1 + buyMarket.TakerFee));
        }

        private double Tradable(OrderBook buyMarket, OrderBook sellMarket, ITrader trader)
        { // 호가에 있는 금액(95퍼만) , 총 잔액(95퍼만)으로 해당 호가의 가격으로 거래 가능한 갯수 중 작은 값
            //return Math.Min(
            //    Math.Min(AskQuantity[0] * 0.95, (TradableTotal(ORDERBOOK_SIDE.ASK, BuyMarket) * 0.95) / AskPrice[0]),
            //    Math.Min(BidQuantity[0] * 0.95, (TradableTotal(ORDERBOOK_SIDE.BID, SellMarket) * 0.95) / BidPrice[0]));
            return Math.Min(
                        Math.Min(sellMarket.DepthQuantity(ORDER_SIDE.buy)[0] * 0.95,
                                    (trader.GetBalance(sellMarket.Market).TotalBalance_USDT() * 0.95) / sellMarket.DepthPrice(ORDERBOOK_SIDE.ASK)[0] * 0.95),
                        Math.Min(buyMarket.DepthQuantity(ORDER_SIDE.sell)[0] * 0.95,
                                    (trader.GetBalance(buyMarket.Market).TotalBalance_USDT() * 0.95) / buyMarket.DepthPrice(ORDERBOOK_SIDE.BID)[0] * 0.95));
        }

        private Data CreateOrderData(OrderBook buyMarket, OrderBook sellMarket)
        {
            Data data = new Data();

            for (int i = 0; i < Constants.ORDERBOOK_SIZE; i++)
            {
                data.setDepth(ORDERBOOK_SIDE.ASK,
                    buyMarket.DepthPrice(ORDER_SIDE.sell)[i],
                    buyMarket.DepthQuantity(ORDER_SIDE.sell)[i],
                    i);

                data.setDepth(ORDERBOOK_SIDE.BID,
                    sellMarket.DepthPrice(ORDER_SIDE.buy)[i],
                    sellMarket.DepthQuantity(ORDER_SIDE.buy)[i],
                    i);
            }

            data.BuyMarket = buyMarket.Market;
            data.BuyMarketTakerFee = buyMarket.TakerFee;
            data.BuyMarketOrderUnit = buyMarket.OrderUnit;
            data.BuyMarketMinTradeValue = buyMarket.MinTradeValue;
            data.BuyMarketBound = buyMarket.Bound;
            data.BuyMarketSymbol = buyMarket.Symbol;
            data.Tradable_USDT[(int)ORDERBOOK_SIDE.ASK] = buyMarket.Tradable_USDT[(int)ORDERBOOK_SIDE.ASK];

            data.SellMarket = sellMarket.Market;
            data.SellMarketTakerFee = sellMarket.TakerFee;
            data.SellMarketOrderUnit = sellMarket.OrderUnit;
            data.SellMarketMinTradeValue = sellMarket.MinTradeValue;
            data.SellMarketBound = sellMarket.Bound;
            data.SellMarketSymbol = sellMarket.Symbol;
            data.Tradable_USDT[(int)ORDERBOOK_SIDE.BID] = sellMarket.Tradable_USDT[(int)ORDERBOOK_SIDE.BID];

            // Common Temporary
            data.MarginThreshold = sellMarket.MarginThreshold;
            data.PriceDiffThreshold = sellMarket.PriceDiffThreshold;

            return data;
        }

        public double Quantity(OrderBook buyMarket, OrderBook sellMarket, ITrader trader)
        {
            double qty = this.Tradable(buyMarket, sellMarket, trader);
            if ((buyMarket.Market.Equals(COIN_MARKET.BINANCE) && (qty * buyMarket.DepthPrice(ORDER_SIDE.sell)[0] < 5))
                || (sellMarket.Market.Equals(COIN_MARKET.BINANCE) && (qty * sellMarket.DepthPrice(ORDER_SIDE.buy)[0] < 5)))
            {
                return 0;
            }
            return CommonApi.cutDecimalNumber(qty, Math.Min(buyMarket.DecimalLength, sellMarket.DecimalLength));
        }

        public double TotalMargin(OrderBook buyMarket, OrderBook sellMarket, ITrader trader)
        {
            double qty = this.Quantity(buyMarket, sellMarket, trader);

            double tMargin = qty * sellMarket.DepthPrice(ORDER_SIDE.buy)[0] * (1 - sellMarket.TakerFee)
                - qty * buyMarket.DepthPrice(ORDER_SIDE.sell)[0] * (1 + buyMarket.TakerFee);
            return tMargin;
        }
    }
}