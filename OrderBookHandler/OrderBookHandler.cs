namespace OrderBookHandler
{
    using Configuration;
    using DataModels;
    using Interfaces;
    using LogTrace.Interfaces;
    using Markets.Interfaces;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using TidGenerators;
    using Traders;
    using Workers;
    using Workers.Interfaces;

    public class OrderBookHandler : IOrderbookSubscriber, IObservableOrderbooks
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("OrderBookControlLogger");

        /// <summary>
        /// / Temporary
        /// </summary>
        public enum OPERATION_MODE
        { START_AND_GET_ORDERBOOK = 0, CHECK_MARGIN_AND_ORDER, WAIT_FOR_ORDER, WAIT_FOR_ORDERBOOK, CHECK_ORDER_INFO, HANDLE_PENDING_ORDER, DISABLED }

        private object lockObj = new object();

        private IList<IMarket> myMarkets;

        private IDictionary<COIN_MARKET, Queue<double>> syncQueue;

        private ConcurrentQueue<COIN_TYPE> jobQueue;

        private AutoResetEvent getOrderbooksEvent;

        private Trader myTrader;

        private IList<OrderBook> myOrderBookList;

        private ConcurrentDictionary<string, OrderBook> myTickerboard;

        private ConcurrentQueue<OrderBook> myJobQueue;

        private IDictionary<string, IWorker> myWorkers;

        private IList<IObserverOrderbooks> myObservers;

        private int myOrderBookId = 0;

        private Task myNotifyOrderbookTask;

        private AutoResetEvent stopEvent;

        public OrderBookHandler()
        {
            this.myObservers = new List<IObserverOrderbooks>();
            this.myMarkets = new List<IMarket>();
            this.myTickerboard = new ConcurrentDictionary<string, OrderBook>();
            this.myWorkers = new Dictionary<string, IWorker>();
            this.syncQueue = new Dictionary<COIN_MARKET, Queue<double>>();
            this.jobQueue = new ConcurrentQueue<COIN_TYPE>();
            this.getOrderbooksEvent = new AutoResetEvent(false);
            this.myOrderBookList = new List<OrderBook>();

            this.stopEvent = new AutoResetEvent(false);

            this.myNotifyOrderbookTask = Task.Run(() => this.NotifyOrderbook());
            this.myJobQueue = new ConcurrentQueue<OrderBook>();
        }

        ~OrderBookHandler()
        {
            this.stopEvent.Set();
        }

        public bool RequestOrderBook(string coinType)
        {
            JObject paramsObj = new JObject();

            paramsObj.Add("symbol", coinType);

            foreach (IWorker worker in this.myWorkers.Values)
            {
                worker.Requset(REQUEST_TYPE.ORDERBOOK, paramsObj.ToString());
            }

            return true;
        }

        // Will be Refact BalanceControl
        public bool GetBalanceAndPosition(COIN_MARKET marketName, Trader trader)
        {
            IMarket market = null;
            foreach (IMarket m in this.myMarkets)
            {
                if (m.GetMyMarketName().Equals(marketName))
                {
                    market = m;
                    break;
                }
            }

            if (market.Equals(null))
            {
                throw new NotImplementedException("NO HAVE MARKET");
            }

            List<AutoResetEvent> doneList = new List<AutoResetEvent>();
            this.myTrader = trader;

            doneList.Add(market.GetBalance());
            doneList.Add(market.GetPosition());

            WaitHandle.WaitAll(doneList.ToArray(), -1, false);
            return true;
        }

        public string SetLeverage(COIN_MARKET marketName, int leverage)
        {
            IMarket market = null;
            foreach (IMarket m in this.myMarkets)
            {
                if (m.GetMyMarketName().Equals(marketName))
                {
                    foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                    {
                        AutoResetEvent done = m.SetLeverage(COIN_TYPE.BTC, leverage);
                        done.WaitOne();

                        Thread.Sleep(1000);
                    }
                    break;
                }
            }

            return "20";
        }

        public void RegisterMarket(IMarket market)
        {
            if (myMarkets.Contains(market))
            {
                return;
            }

            market.Subscribe(this as IOrderbookSubscriber);
            myMarkets.Add(market);

            if (!this.myTickerboard.ContainsKey(market.GetMyMarketName().ToString()))
            {
                this.myTickerboard.TryAdd(market.GetMyMarketName().ToString(), OrderBook.Empty());

                IWorker worker = new RESTAPIWorker(market);
                this.myWorkers.Add(market.GetMyMarketName().ToString(), worker);

                worker.Start();
            }
        }

        public void UnregisterMarket(IMarket market)
        {
            if (myMarkets.Contains(market))
            {
                market.Unsubscribe(this as IOrderbookSubscriber);
                myMarkets.Remove(market);

                if (this.myTickerboard.ContainsKey(market.GetMyMarketName().ToString()))
                {
                    OrderBook dummy;
                    this.myTickerboard.TryRemove(market.GetMyMarketName().ToString(), out dummy);

                    IWorker worker;
                    if (this.myWorkers.TryGetValue(market.GetMyMarketName().ToString(), out worker))
                    {
                        worker.Stop();
                    }

                    this.myWorkers.Remove(market.GetMyMarketName().ToString());
                }
            }
        }

        public void PublishOrderbook(OrderBook orderBook)
        {
            this.myJobQueue.Enqueue(orderBook);
        }

        public void NotifyOrderbook()
        {
            while (!this.stopEvent.WaitOne(1))
            {
                if (!this.myJobQueue.IsEmpty)
                {
                    OrderBook orderBook;
                    if (this.myJobQueue.TryDequeue(out orderBook))
                    {
                        Stopwatch timer = new Stopwatch();
                        timer.Start();

                        this.myTickerboard[orderBook.Market.ToString()] = orderBook;
                        int curId = TidGenerator.GenerateTransactionId();

                        IList<OrderBook> buyOrderBook = new List<OrderBook>();
                        IList<OrderBook> sellOrderBook = new List<OrderBook>();

                        foreach (OrderBook item in this.myTickerboard.Values)
                        {
                            if (item.State.Equals(REQUEST_STATE.NORMAL))
                            {
                                buyOrderBook.Add(item);
                                sellOrderBook.Add(item);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        foreach (IObserverOrderbooks ob in this.myObservers)
                        {
                            ob.Publish(new OrderBooks(curId, buyOrderBook, sellOrderBook));
                        }

                        timer.Stop();
                        if (curId % 1000 == 0)
                        {
                            myLogger.Info($"OrderId : {curId}, ElspTime : {timer.ElapsedMilliseconds} ms");
                        }
                    }
                }
            }
        }

        public void Subscribe(IObserverOrderbooks subscriber)
        {
            if (this.myObservers.Contains(subscriber))
            {
                return;
            }

            this.myObservers.Add(subscriber);
        }

        public void Unsubscribe(IObserverOrderbooks subscriber)
        {
            if (this.myObservers.Contains(subscriber))
            {
                this.myObservers.Remove(subscriber);
            }
        }
    }
}