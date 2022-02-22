namespace CalculationEngine
{
    using CalculationEngine.LegacyStrategies;
    using CalculationEngine.Strategies;
    using Configuration;
    using DataModels;
    using LogTrace.Interfaces;
    using OrderBookHandler.Interfaces;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Traders.Interfaces;

    public class CalculationStrategy : IObserverOrderbooks
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CalcLogger");

        private readonly BackgroundWorker worker;

        private IDictionary<COIN_MARKET, Balance> myBalances;

        private IList<ITrader> myTraders;

        private CheckValidity myLegacyAlg;

        private Task myNotifyOrderbookTask;

        private AutoResetEvent stopEvent;

        private ConcurrentQueue<OrderBooks> myJobQueue;

        private ManagedOrderCycleAlg myOrderCycleAlg;

        public CalculationStrategy()
        {
            this.myTraders = new List<ITrader>();
            this.myLegacyAlg = new CheckValidity();
            this.myBalances = new Dictionary<COIN_MARKET, Balance>();

            this.stopEvent = new AutoResetEvent(false);

            this.myJobQueue = new ConcurrentQueue<OrderBooks>();
            this.myOrderCycleAlg = new ManagedOrderCycleAlg();
        }

        ~CalculationStrategy()
        {
            this.stopEvent.Set();
        }

        public void PublishBalances(COIN_MARKET market, Balance balance)
        {
            if (!this.myBalances.ContainsKey(market))
            {
                this.myBalances.Add(market, balance);
                return;
            }

            this.myBalances[market] = balance;
        }

        public void Publish(OrderBooks orderbooks)
        {
            /// Temporary
            /// Only one trader
            AlgManager.Instance.UpdateOrderBook(orderbooks);
            this.myJobQueue.Enqueue(orderbooks);
        }

        public void RegisterTrader(ITrader trader)
        {
            if (!this.myTraders.Contains(trader))
            {
                this.myTraders.Add(trader);

                // Temporary
                foreach (COIN_MARKET market in Enum.GetValues(typeof(COIN_MARKET)))
                {
                    this.PublishBalances(market, trader.GetBalance(market));
                }
            }
        }

        public void UnregisterTrader(ITrader trader)
        {
            if (this.myTraders.Contains(trader))
            {
                this.myTraders.Remove(trader);
            }
        }

        public void Start()
        {
            this.myNotifyOrderbookTask = new Task(this.CalcMargin);
            this.myNotifyOrderbookTask.Start();
        }

        private void CalcMargin()
        {
            while (!this.stopEvent.WaitOne(1))
            {
                try
                {
                    if (!this.myJobQueue.IsEmpty)
                    {
                        OrderBooks orderBooks;
                        if (this.myJobQueue.TryDequeue(out orderBooks))
                        {
                            Data result;
                            if (this.myLegacyAlg.checkMargin(orderBooks, myTraders.First(), out result))
                            {
                                myLogger.Debug("GET MARGIN");

                                this.myOrderCycleAlg.RequestOrder(new Tuple<ITrader, Data>(this.myTraders.First(), result));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    myLogger.Error($"Error CallStack Calc Margin : {e.StackTrace}");
                }
            }
        }
    }
}