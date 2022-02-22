namespace CalculationEngine
{
    using CalculationEngine.Algorithm;
    using Configuration;
    using DataModels;
    using Traders.Interfaces;

    public class AlgManager
    {
        private static AlgManager instance = null;

        private static object lockObj = new object();

        public static AlgManager Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new AlgManager();
                    }

                    return instance;
                }
            }
        }

        private OrderBooks myLatestOrderBook;

        private AlgManager()
        {
        }

        public void UpdateOrderBook(OrderBooks orderBooks)
        {
            this.myLatestOrderBook = orderBooks;
        }

        public Order FindBestPriceForReorder(ITrader trader, ORDER_SIDE side, double remainQty, Order originOrder)
        {
            FindBestPriceForReOrder calcOrder = new FindBestPriceForReOrder();
            calcOrder.UpdateOrderbook(this.myLatestOrderBook);

            return calcOrder.Calc(trader, side, remainQty, originOrder);
        }

        public Order AppliedAREEPAlg(ITrader trader, Order originOrder)
        {
            AppliedAREEPAlg calcOrder = new AppliedAREEPAlg();

            return calcOrder.Calc(trader, originOrder);
        }

        public Data AlignOrderUnit(ITrader trader, Data originOrder)
        {
            AlignOrderUnit calcOrder = new AlignOrderUnit();

            return calcOrder.Calc(trader, originOrder);
        }
    }
}