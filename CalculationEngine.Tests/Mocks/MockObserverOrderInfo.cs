namespace CalculationEngine.Tests.Mocks
{
    using DataModels;
    using Markets.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class MockObserverOrderInfo : IOrderInfoSubscriber
    {
        private IList<OrderInfo> myOrderInfos;

        public MockObserverOrderInfo()
        {
            this.myOrderInfos = new List<OrderInfo>();
        }

        public int CurrentOrderBooksId()
        {
            if (this.myOrderInfos.Count == 0)
            {
                return 0;
            }

            return this.myOrderInfos.Last().OrderbookId;
        }

        public OrderInfo GetOrderInfo()
        {
            return this.myOrderInfos.Last();
        }

        public IList<OrderInfo> GetOrderInfos()
        {
            return this.myOrderInfos;
        }

        public void PublishOrderInfo(OrderInfo orderInfo)
        {
            myOrderInfos.Add(orderInfo);
        }
    }
}