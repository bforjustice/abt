namespace DataModels
{
    using Common;
    using Configuration;
    using LogTrace.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PendingInfo
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("PendingInfoLogger");

        public PendingInfo()
        {
            if (PendingHistoryList == null)
            {
                PendingHistoryList = new List<PendingHistory>();
            }
        }

        public int RetryCount { get; set; } = 0;

        public bool Finished { get; set; } = false;

        public int[] FailCountEachMarket { get; set; } = new int[Constants.MARKET_COUNT];

        public List<PendingHistory> PendingHistoryList { get; set; } = null;

        public void AddPendingHistory(COIN_MARKET coinMarket, double acceptedQty, double avgPrice)
        {
            PendingHistory pendingHistory = new PendingHistory();
            pendingHistory.Market = coinMarket;
            pendingHistory.AcceptedQty = acceptedQty;

            pendingHistory.AvgPrice = avgPrice;

            PendingHistoryList.Add(pendingHistory);

            double totalQty = 0;
            myLogger.Warn("--------------------");
            foreach (PendingHistory history in PendingHistoryList)
            {
                myLogger.Warn($"market = {history.Market}");
                myLogger.Warn($"qty = {history.AcceptedQty}");
                totalQty += history.AcceptedQty;
                myLogger.Warn($"avg Price = {history.AvgPrice}");
            }
            myLogger.Warn($"totalQty = {totalQty}");
            myLogger.Warn("--------------------");
        }

        public void RemoveLastHistory()
        {
            if (PendingHistoryList.Count > 0)
            {
                PendingHistoryList.Remove(PendingHistoryList.Last());
            }
        }

        public COIN_MARKET InitialMarket { get; set; }

        public COIN_MARKET MostMarket
        {
            get
            {
                double[] totalQty = new double[Constants.MARKET_COUNT];
                double max = 0;
                COIN_MARKET mostMarket = LastOrderMarket;
                foreach (PendingHistory history in PendingHistoryList)
                {
                    totalQty[(int)history.Market] += history.AcceptedQty;
                }

                foreach (var coinMarket in Enum.GetValues(typeof(COIN_MARKET)))
                {
                    if (totalQty[(int)coinMarket] > max)
                    {
                        mostMarket = (COIN_MARKET)coinMarket;
                    }
                }
                return mostMarket;
            }
        }

        public double AvgPriceForMargin
        {
            get
            {
                double totalQty = 0;
                double totalAmount = 0;
                foreach (PendingHistory history in PendingHistoryList)
                {
                    if (OrderSide.Equals(ORDER_SIDE.sell))
                    {
                        //// Todo
                        //totalAmount += history.AvgPrice * history.AcceptedQty * (1 - Settings.TakerFee[(int)history.Market]);
                        //myLogger.Debug("AvgPriceForMargin", $"sell {totalAmount}, {history.AvgPrice}, {history.AcceptedQty}, {Settings.TakerFee[(int)history.Market]}");
                    }
                    else
                    {
                        //// Todo
                        //totalAmount += history.AvgPrice * history.AcceptedQty * (1 + Settings.TakerFee[(int)history.Market]);
                        //myLogger.Debug("AvgPriceForMargin", $"buy {totalAmount}, {history.AvgPrice}, {history.AcceptedQty}, {Settings.TakerFee[(int)history.Market]}");
                    }
                    totalQty += history.AcceptedQty;
                }

                myLogger.Debug($"buy {totalAmount}, {totalQty}");
                return (totalAmount / totalQty);
            }
        }

        public double AvgPriceOriginal
        {
            get
            {
                double totalQty = 0;
                double totalPrice = 0;
                foreach (PendingHistory history in PendingHistoryList)
                {
                    totalPrice += history.AvgPrice * history.AcceptedQty;

                    totalQty += history.AcceptedQty;
                }
                return (totalPrice / totalQty);
            }
        }

        public double TotalFilledQty
        {
            get
            {
                double totalQty = 0;
                foreach (PendingHistory history in PendingHistoryList)
                {
                    totalQty += history.AcceptedQty;
                }
                myLogger.Debug($"{totalQty}");
                return totalQty;
            }
        }

        public double TotalQty { get; set; } = 0;

        public double LastOrderPrice { get; set; } = 0;

        public double LastOrderQty { get; set; } = 0;

        public COIN_MARKET LastOrderMarket { get; set; }

        public string TradeType { get; set; } = string.Empty;

        public string OrderId { get; set; } = string.Empty;

        public long Timestamp { get; set; } = 0;

        public long LastAlreadyCheckTime { get; set; } = 0;
        public string Code { get; set; } = string.Empty;

        public bool Failed { get; set; } = false;

        public ORDER_SIDE OrderSide { get; set; }

        public long TimeDiff
        {
            get { return TimeManager.UtcTimeMS() - Timestamp; }
        }

        public double OriginalSellPrice { get; set; } = 0;

        public double OriginalBuyPrice { get; set; } = 0;
    }

    public class PendingInfoList
    {
        public static List<PendingInfo> listData = new List<PendingInfo>();
    }

    public class PendingHistory
    {
        public COIN_MARKET Market { get; set; }

        public double AcceptedQty { get; set; } = 0;

        public double AvgPrice { get; set; } = 0;
    }
}