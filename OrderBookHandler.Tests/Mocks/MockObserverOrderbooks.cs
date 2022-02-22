namespace OrderBookHandler.Tests.Mocks
{
    using DataModels;
    using Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class MockObserverOrderbooks : IObserverOrderbooks
    {
        private IList<OrderBooks> myOrderbooks;

        public MockObserverOrderbooks()
        {
            this.myOrderbooks = new List<OrderBooks>();
        }

        public void Publish(OrderBooks orderbooks)
        {
            myOrderbooks.Add(orderbooks);
        }

        public int CurrentOrderBooksId()
        {
            if (this.myOrderbooks.Count == 0)
            {
                return 0;
            }

            return this.myOrderbooks.Last().OrderbookId;
        }
    }
}