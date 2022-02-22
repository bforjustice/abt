namespace DataUpdater
{
    using Common;
    using Configuration;
    using Database;
    using Database.Interfaces;
    using DataModels;
    using DataUpdater.Interfaces;
    using LogTrace.Interfaces;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class TradeDateUpdater : IObserverTrade
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CommLogger");

        private IDataBaseStoreController myDBCtrl;

        private AutoResetEvent stopEvent;

        private ConcurrentQueue<Tuple<ConcurrentStack<OrderInfo>, ConcurrentStack<OrderInfo>>> myJobQueue;

        public TradeDateUpdater()
        {
            this.myDBCtrl = new DBController();
            this.stopEvent = new AutoResetEvent(false);
            this.myJobQueue = new ConcurrentQueue<Tuple<ConcurrentStack<OrderInfo>, ConcurrentStack<OrderInfo>>>();

            Task.Run(() => StoreTradeDatas());
        }

        ~TradeDateUpdater()
        {
            this.stopEvent.Set();
        }

        public void Publish(ConcurrentStack<OrderInfo> buySide, ConcurrentStack<OrderInfo> sellSide)
        {
            this.myJobQueue.Enqueue(new Tuple<ConcurrentStack<OrderInfo>, ConcurrentStack<OrderInfo>>(buySide, sellSide));
        }

        public void StoreTradeDatas()
        {
            while (!this.stopEvent.WaitOne(1))
            {
                if (!this.myJobQueue.IsEmpty)
                {
                    Tuple<ConcurrentStack<OrderInfo>, ConcurrentStack<OrderInfo>> tradeInfo;
                    if (this.myJobQueue.TryDequeue(out tradeInfo))
                    {
                        OrderInfo buyInfo;
                        OrderInfo sellInfo;

                        tradeInfo.Item1.TryPeek(out buyInfo);
                        tradeInfo.Item2.TryPeek(out sellInfo);

                        IDictionary<string, string> buyInfos = new Dictionary<string, string>();
                        IDictionary<string, string> sellInfos = new Dictionary<string, string>();

                        // Temporary
                        double usdtMargin = (Math.Abs(sellInfo.FilledQty) * (sellInfo.AvgPrice.Equals(0.0) ? sellInfo.EntryPrice : sellInfo.AvgPrice)) -
                            (Math.Abs(buyInfo.FilledQty) * (buyInfo.AvgPrice.Equals(0.0) ? buyInfo.EntryPrice : buyInfo.AvgPrice));

                        buyInfos.Add("@Buy_Original_Market_Name", buyInfo.Market.ToString());
                        buyInfos.Add("@Buy_Market_Name", buyInfo.Market.ToString());
                        buyInfos.Add("@Buy_Coin_Quantity", CommonApi.cutDecimalNumber(Math.Abs(buyInfo.FilledQty), 4).ToString());
                        buyInfos.Add("@Buy_Coin_Price", buyInfo.AvgPrice.Equals(0.0) ? buyInfo.EntryPrice.ToString() : buyInfo.AvgPrice.ToString());
                        buyInfos.Add("@Buy_Coin_Original_Price", buyInfo.EntryPrice.ToString());
                        buyInfos.Add("@Buy_Trade_Type", this.CreateTagInfo(tradeInfo.Item1));

                        sellInfos.Add("@Sell_Original_Market_Name", sellInfo.Market.ToString());
                        sellInfos.Add("@Sell_Market_Name", sellInfo.Market.ToString());
                        sellInfos.Add("@Sell_Coin_Quantity", CommonApi.cutDecimalNumber(Math.Abs(sellInfo.FilledQty), 4).ToString());
                        sellInfos.Add("@Sell_Coin_Price", sellInfo.AvgPrice.Equals(0.0) ? sellInfo.EntryPrice.ToString() :sellInfo.AvgPrice.ToString());
                        sellInfos.Add("@Sell_Coin_Original_Price", sellInfo.EntryPrice.ToString());
                        sellInfos.Add("@Sell_Trade_Type", this.CreateTagInfo(tradeInfo.Item2));

                        this.myDBCtrl.StoreCompletedTradeLog(LOG_TYPE.MARGIN, buyInfo.CoinType, buyInfos, sellInfos, usdtMargin, string.Empty);
                    }
                }
            }
        }

        private string CreateTagInfo(ConcurrentStack<OrderInfo> infos)
        {
            string result = string.Empty;

            int cnt = 0;

            IList<OrderInfo> completeList = new List<OrderInfo>();

            while (!infos.IsEmpty)
            {
                OrderInfo info;
                if (infos.TryPop(out info))
                {
                    myLogger.Warn($"\n=================== Stack Num : {(++cnt).ToString()}=======================\n");
                    myLogger.Warn($"\n{info.ToString()}");
                    myLogger.Warn($"=============================================================================");

                    if (info.Type.Equals(REQUEST_TYPE.OPEN_ORDER))
                    {
                        if (info.PendingType.Equals(PENDING_TYPE.COMPLETE) || 
                            info.PendingType.Equals(PENDING_TYPE.CANCELED))
                        {
                            completeList.Add(info);
                        }
                    }
                }
            }

            if (completeList.Count.Equals(1))
            {
                result = "S";
            }
            else
            {
                foreach (OrderInfo info in completeList)
                {
                    result += "A";
                }
            }

            
            return result;
        }

        private string CreateTradeLog(OrderInfo info)
        {
            return info.ToString();
        }
    }
}