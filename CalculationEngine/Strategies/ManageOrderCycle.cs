namespace CalculationEngine.Strategies
{
    using CalculationEngine.Strategies.SubStrategies.Interfaces;
    using Common;
    using Configuration;
    using DataModels;
    using LogTrace.Interfaces;
    using Markets.Interfaces;
    using System;
    using System.Collections.Concurrent;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Traders.Interfaces;
    using static CalculationEngine.Strategies.SubStrategies.Interfaces.OrdererFactory;

    public class ManageOrderCycle : IOrderbookSubscriber, IOrderInfoSubscriber, IDisposable
    {
        private static ILogger myLogger;

        private ORDER_SIDE myOrderSide;

        private Order myOrder;

        private Order myModifiedOrder;

        private ITrader myTrader;

        private AutoResetEvent myStopEvent;

        private OrderBook myLatestOrderbook;

        private ConcurrentStack<OrderInfo> myOrderInfoHistory;

        private ConcurrentQueue<OrderInfo> myOrderInfoQueue;

        private ConcurrentStack<PendingInfo> myPendingInfos;

        private ORDER_INFO_STATE myLastOrderState;

        private int StndRetryCnt = 10;

        private int myRetryCnt = 0;

        public ORDER_INFO_STATE LastOrderState => this.myLastOrderState;


        public ManageOrderCycle(ORDER_SIDE side, Order order, ITrader trader)
        {
            string loggerName = side.Equals(ORDER_SIDE.buy) ? "BuyMarketLogger" : "SellMarketLogger";
            myLogger = LogTraceService.Instance.GetLogger(loggerName);

            this.myLastOrderState = ORDER_INFO_STATE.INIT;
            this.myOrderSide = side;
            this.myOrder = order;
            this.myTrader = trader;
            this.myStopEvent = new AutoResetEvent(false);
            this.myOrderInfoHistory = new ConcurrentStack<OrderInfo>();
            this.myOrderInfoQueue = new ConcurrentQueue<OrderInfo>();
            this.myPendingInfos = new ConcurrentStack<PendingInfo>();
        }

        ~ManageOrderCycle()
        {
            this.Dispose();
        }

        public ConcurrentStack<OrderInfo> OrderInfoHistory => this.myOrderInfoHistory;

        public void Dispose()
        {
            this.myTrader.UnsubscribeOrderBook(this.myOrder.Market, this);
            this.myTrader.UnsubscribeOrderInfo(this.myOrder.Market, this);
        }

        public void PublishOrderbook(OrderBook orderBook)
        {
            this.myLatestOrderbook = orderBook;
        }

        public void PublishOrderInfo(OrderInfo orderInfo)
        {
            this.myOrderInfoQueue.Enqueue(orderInfo);
        }

        public Task Start()
        {
            myLogger.Info($"\n================ Start Trade [{this.myOrder.Market.ToString()}]=====================\n");
            myLogger.Info($"{this.myOrder.ToString()}");
            myLogger.Info("\n================================================\n");

            //this.myModifiedOrder = AlgManager.Instance.AppliedAREEPAlg(this.myTrader, this.myOrder);
            this.myModifiedOrder = this.myOrder.Clone();

            myLogger.Info($"\n================ Applied AREE [{this.myOrder.Market.ToString()}]=====================\n");
            myLogger.Info($"Modified Qty : \n{this.myModifiedOrder.ToString()}");
            myLogger.Info("\n================================================\n");

            this.myTrader.SubscribeOrderBook(this.myModifiedOrder.Market, this);
            this.myTrader.SubscribeOrderInfo(this.myModifiedOrder.Market, this);

            return Task.Run(() => this.DoWork());
        }

        private void DoWork()
        {
            try
            {
                this.myTrader.RequestOrder(this.myModifiedOrder).WaitOne();

                while (!this.myStopEvent.WaitOne(10))
                {
                    OrderInfo currentInfo;
                    while (this.myOrderInfoQueue.TryDequeue(out currentInfo))
                    {
                        PendingInfo currentPendingInfo = this.makeDefaultPendingInfo();
                        ORDER_INFO_STATE state = this.DeterminantOrderInfo(currentInfo, currentPendingInfo);

                        if (state.Equals(ORDER_INFO_STATE.SUCCESS))
                        {
                            this.myStopEvent.Set();
                        }
                        else if (state.Equals(ORDER_INFO_STATE.NEED_ORDER_CHECK))
                        {
                            // Wait to Change order State
                            // time stride 1000ms, 10 times
                            Thread.Sleep(1000);

                            myLogger.Info($"Retry Cnt {this.myRetryCnt.ToString()}");
                        }

                        if (currentInfo.OrderId.Equals(string.Empty))
                        {
                            OrderInfo lastOrderInfo;
                            if (this.myOrderInfoHistory.TryPeek(out lastOrderInfo))
                            {
                                currentInfo.OrderId = lastOrderInfo.OrderId;
                            }
                            else
                            {
                                throw new ArgumentNullException();
                            }
                        }

                        if (currentInfo.RemainQty.Equals(0))
                        {
                            currentInfo.RemainQty = this.FindRemainQty(this.myOrderInfoHistory);
                        }

                        // Cancel or Finish or Retry
                        IOrderer orderer = OrdererFactory.CreateOrderer(
                            state, 
                            this.myTrader, 
                            currentInfo, 
                            currentPendingInfo, 
                            this, 
                            this.myModifiedOrder);

                        orderer.DoWork();

                        this.myOrderInfoHistory.Push(currentInfo);
                    }
                }
            }
            catch (Exception e)
            {
                myLogger.Error($"Error ManagedOrderCycle : {e.StackTrace}");
                this.Dispose();
                throw new InvalidOperationException();
            }
        }

        private ORDER_INFO_STATE DeterminantOrderInfo(OrderInfo orderInfo, PendingInfo info)
        {
            ORDER_INFO_STATE result = ORDER_INFO_STATE.SUCCESS;

            if (orderInfo.Type.Equals(REQUEST_TYPE.PLACE_ORDER))
            {
                myLogger.Info(this.PrintOrderInfo("Need OrderInfos", orderInfo));
                return ORDER_INFO_STATE.NEED_ORDER_CHECK;
            }

            if ((orderInfo.PendingType).Equals(PENDING_TYPE.COMPLETE))
            {
                myLogger.Info(this.PrintOrderInfo("Completed Order", orderInfo));
                info.AddPendingHistory(orderInfo.Market, orderInfo.FilledQty, orderInfo.AvgPrice);
                info.Finished = true;
            }
            else if (orderInfo.PendingType.Equals(PENDING_TYPE.PARTIAL) ||
                        orderInfo.PendingType.Equals(PENDING_TYPE.FULL) ||
                        orderInfo.PendingType.Equals(PENDING_TYPE.NEW))
            {
                if (this.myRetryCnt < this.StndRetryCnt)
                {
                    this.myRetryCnt++;
                    result = ORDER_INFO_STATE.NEED_ORDER_CHECK;
                }
                else
                {
                    myLogger.Info(this.PrintOrderInfo("Need Cancelled Order", orderInfo));
                    info.Finished = false;
                    info.Failed = false;

                    result = ORDER_INFO_STATE.NEED_CANCELED;
                }
            }
            else if ((orderInfo.PendingType.Equals(PENDING_TYPE.CANCELED)))
            {
                this.myRetryCnt = 0;

                myLogger.Info(this.PrintOrderInfo("Cancel Order", orderInfo));
                info.Finished = false;
                info.Failed = false;

                result = ORDER_INFO_STATE.NEED_ORDER_CHECK;
            }
            else if ((orderInfo.PendingType.Equals(PENDING_TYPE.NEW)))
            {
                this.myRetryCnt++;

                myLogger.Info(this.PrintOrderInfo("Need to more check order State : New", orderInfo));
                result = ORDER_INFO_STATE.NEED_ORDER_CHECK;
            }
            else if ((orderInfo.PendingType.Equals(PENDING_TYPE.CANCEL_AND_REORDER)))
            {
                this.myRetryCnt = 0;

                myLogger.Info(this.PrintOrderInfo("Cancel Complete and Reorder", orderInfo));
                result = ORDER_INFO_STATE.RETRY_ORDER;
            }
            else
            {
                myLogger.Info(this.PrintOrderInfo("UNKNOWN State", orderInfo));
                result = ORDER_INFO_STATE.ORDER_FAILED;
            }

            return result;
        }

        private string PrintOrderInfo(string msg, OrderInfo info)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"\n========================================================================\n");
            builder.Append(msg + "\n");
            builder.Append($"{info.ToString()}\n");
            builder.Append($"========================================================================\n");

            return builder.ToString();
        }

        private string CurrentOrderInfoMessage(OrderInfo info)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"\n==== Crt Order Info ====\n");
            builder.Append($"Market       : {info.Market.ToString()}\n");
            builder.Append($"PendingType  : {info.PendingType.ToString()}\n");
            builder.Append($"========================\n");

            return builder.ToString();
        }

        private PendingInfo makeDefaultPendingInfo()
        {
            string code = TimeManager.SyncronizedTime.ToString("yyyyMMddHHmmssfff");

            long currentTime = TimeManager.UtcTimeMS();

            return new PendingInfo()
            {
                OriginalBuyPrice = this.myOrder.OrderPrice,
                InitialMarket = this.myOrder.Market,
                OrderSide = this.myOrder.Side,
                Code = code,
                LastOrderMarket = this.myOrder.Market,
                LastOrderPrice = this.myOrder.OrderPrice,
                LastOrderQty = this.myOrder.Quantity,
                TotalQty = this.myOrder.Quantity,
                Timestamp = currentTime,
            };
        }

        private double FindRemainQty(ConcurrentStack<OrderInfo> orderInfos)
        {
            ConcurrentStack<OrderInfo> tDummy = new ConcurrentStack<OrderInfo>();

            OrderInfo dummy;
            double remainQty = 0;
            while (!orderInfos.IsEmpty)
            {
                if (orderInfos.TryPop(out dummy))
                {
                    tDummy.Push(dummy);
                    if (!dummy.RemainQty.Equals(0))
                    {
                        remainQty = dummy.RemainQty;
                        break;
                    }
                }
                else
                {
                    myLogger.Error("Not Found RemainQty");
                    throw new InvalidOperationException();
                }
            }

            while (!tDummy.IsEmpty)
            {
                if (!tDummy.TryPop(out dummy))
                {
                    orderInfos.Push(dummy);
                }
            }

            myLogger.Warn($"Find Remain Qty : {remainQty}");

            return remainQty;
        }
    }
}