namespace CalculationEngine.Strategies
{
    using Configuration;
    using DataModels;
    using DataUpdater;
    using LogTrace.Interfaces;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Traders.Interfaces;

    public class ManagedOrderCycleAlg : IDisposable
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CalcLogger");

        private AutoResetEvent stopEvent;

        private ConcurrentQueue<Tuple<ITrader, Data>> myJobQueue;

        private Task myNotifyOrderbookTask;

        private IList<ManageOrderPair> myOrders;

        private TradeDateUpdater myTradeDataUpdater;

        private AutoResetEvent syncReqOrder;

        public ManagedOrderCycleAlg()
        {
            this.myTradeDataUpdater = new TradeDateUpdater();
            this.stopEvent = new AutoResetEvent(false);
            this.myJobQueue = new ConcurrentQueue<Tuple<ITrader, Data>>();

            this.myNotifyOrderbookTask = Task.Run(() => this.DoWork());
            this.myOrders = new List<ManageOrderPair>();

            this.syncReqOrder = new AutoResetEvent(false);
        }

        ~ManagedOrderCycleAlg()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.stopEvent.Set();
        }

        public void PublishOrderInfo(OrderInfo info)
        {
            throw new NotImplementedException();
        }

        public IList<ManageOrderPair> GetMyOrders()
        {
            return this.myOrders;
        }

        public void UpdateLatestOrderBook(OrderBooks orderbooks)
        {
            foreach (ManageOrderPair item in this.myOrders)
            {
                item.UpdateOrderbooks(orderbooks);
            }
        }

        public void RequestOrder(Tuple<ITrader, Data> orderTuple)
        {
            ITrader trader = orderTuple.Item1;
            Data tOrderData = orderTuple.Item2;

            this.myJobQueue.Enqueue(orderTuple);
        }

        public void CompletedOrder(ManageOrderPair orderCycle)
        {
            if (this.myOrders.Contains(orderCycle))
            {
                orderCycle.Unsubscribe(this.myTradeDataUpdater);
                this.myOrders.Remove(orderCycle);
            }
        }

        private void DoWork()
        {
            try
            {
                while (!this.stopEvent.WaitOne(1))
                {
                    if (!this.myJobQueue.IsEmpty)
                    {
                        Tuple<ITrader, Data> job;
                        if (this.myJobQueue.TryDequeue(out job))
                        {
                            ITrader trader = job.Item1;
                            Data orderData = job.Item2;

                            // Modified OrderUnit
                            Data modifiedOrderData = AlgManager.Instance.AlignOrderUnit(trader, orderData);

                            // Applied Modified AREEP

                            ManageOrderPair handleOrder = new ManageOrderPair(this, trader, modifiedOrderData);
                            handleOrder.Subscribe(this.myTradeDataUpdater);
                            this.myOrders.Add(handleOrder);

                            handleOrder.Start();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                myLogger.Error($"Eror OrderCycleAlg StackTrace : ${e.StackTrace}");
                this.Dispose();
            }
        }
    }
}