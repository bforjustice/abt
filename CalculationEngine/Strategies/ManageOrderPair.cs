namespace CalculationEngine.Strategies
{
    using Configuration;
    using Database;
    using DataModels;
    using DataUpdater.Interfaces;
    using LogTrace.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Traders.Interfaces;
    using static CalculationEngine.Strategies.SubStrategies.Interfaces.OrdererFactory;

    public class ManageOrderPair : IObservableTrade
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CalcLogger");

        private ITrader myTrader;

        private ManagedOrderCycleAlg myAlg;

        private Data myOrderData;

        private List<Task> myWaitHandle;

        private Task myWaitSignalTask;

        private AutoResetEvent stopEvent;

        private IList<OrderInfo> myOrderInfos;

        private IDictionary<ORDER_SIDE, PendingInfo> myPendingInfos;

        private OrderBooks myLatestOrderbooks;

        private Thread myWaitThread;

        private ManageOrderCycle buyOrderCycle;

        private ManageOrderCycle sellOrderCycle;

        private DBController myDbCtrl;

        private IList<IObserverTrade> myTradeObserver;

        public ManageOrderPair(ManagedOrderCycleAlg managedAlg, ITrader trader, Data orderData)
        {
            this.myTradeObserver = new List<IObserverTrade>();
            this.myDbCtrl = new DBController();
            this.myAlg = managedAlg;
            this.myTrader = trader;
            this.myOrderData = orderData;
            this.stopEvent = new AutoResetEvent(false);
            this.myWaitHandle = new List<Task>();

            this.myOrderInfos = new List<OrderInfo>();
            this.myPendingInfos = new Dictionary<ORDER_SIDE, PendingInfo>();
        }

        ~ManageOrderPair()
        {
            this.myAlg.CompletedOrder(this);

            this.myWaitHandle.Clear();
        }

        public void Subscribe(IObserverTrade subscriber)
        {
            if (this.myTradeObserver.Contains(subscriber))
            {
                return;
            }

            this.myTradeObserver.Add(subscriber);
        }

        public void Unsubscribe(IObserverTrade subscriber)
        {
            if (this.myTradeObserver.Contains(subscriber))
            {
                this.myTradeObserver.Remove(subscriber);
            }
        }

        public void Start()
        {
            try
            {
                this.buyOrderCycle = new ManageOrderCycle(ORDER_SIDE.buy,
                    this.CreateOrder(ORDER_SIDE.buy, this.myOrderData),
                    this.myTrader);
                this.sellOrderCycle = new ManageOrderCycle(ORDER_SIDE.sell,
                    this.CreateOrder(ORDER_SIDE.sell, this.myOrderData),
                    this.myTrader);

                this.myWaitHandle.Add(buyOrderCycle.Start());
                this.myWaitHandle.Add(sellOrderCycle.Start());

                this.myWaitThread = new Thread(new ThreadStart(this.WaitTasks));
                this.myWaitThread.Start();
            }
            catch (Exception e)
            {
                foreach (IObserverTrade sub in this.myTradeObserver)
                {
                    sub.Publish(this.buyOrderCycle.OrderInfoHistory, this.sellOrderCycle.OrderInfoHistory);
                }

                this.myTradeObserver.Clear();
                myLogger.Error($"Error : ManageOrderPair StackTace : {e.StackTrace}");
            }
        }

        public void UpdateOrderbooks(OrderBooks orderbooks)
        {
            this.myLatestOrderbooks = orderbooks;
        }

        private void WaitTasks()
        {
            Task.WaitAll(this.myWaitHandle.ToArray());

            foreach (IObserverTrade sub in this.myTradeObserver)
            {
                sub.Publish(this.buyOrderCycle.OrderInfoHistory, this.sellOrderCycle.OrderInfoHistory);
            }

            this.myTradeObserver.Clear();

            this.myDbCtrl.LoadBalanceAndPosition(this.myTrader);
            this.myAlg.CompletedOrder(this);
        }

        private Order CreateOrder(ORDER_SIDE side, Data rawData)
        {
            Order order = new Order();

            // Common Property
            order.Quantity = rawData.Quantity;
            order.TotalMargin = rawData.TotalMargin;
            order.MarginThreshold = rawData.MarginThreshold;
            order.PriceDiffThreshold = rawData.PriceDiffThreshold;
            order.OrderbookId = rawData.OrderbookId;

            // Market Property
            order.Market = side.Equals(ORDER_SIDE.buy) ? rawData.BuyMarket : rawData.SellMarket;
            order.OrderPrice = side.Equals(ORDER_SIDE.buy) ? rawData.BuyOrderPrice : rawData.SellOrderPrice;
            order.MinTradeValue = side.Equals(ORDER_SIDE.buy) ? rawData.BuyMarketMinTradeValue : rawData.SellMarketMinTradeValue;
            order.OrderUnit = side.Equals(ORDER_SIDE.buy) ? rawData.BuyMarketOrderUnit : rawData.SellMarketOrderUnit;
            order.MarketBound = side.Equals(ORDER_SIDE.buy) ? rawData.BuyMarketBound : rawData.SellMarketBound;
            order.Symbol = side.Equals(ORDER_SIDE.buy) ? rawData.BuyMarketSymbol : rawData.SellMarketSymbol;
            order.Side = side;
            order.Type = ORDER_TYPE.limit;

            return order;
        }
    }
}