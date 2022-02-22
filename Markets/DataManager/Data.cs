namespace Markets.Models
{
    using Configuration;

    public class Data
    {
        public COIN_MARKET BuyMarket { get; set; }

        public double[] Tradable_USDT { get; set; } = new double[Constants.ORDERBOOK_SIDE_COUNT];

        public double[] DepthQuantity(ORDERBOOK_SIDE side)
        {
            return side.Equals(ORDERBOOK_SIDE.BID) ? BidQuantity : AskQuantity;
        }

        public double[] DepthPrice(ORDERBOOK_SIDE side)
        {
            return side.Equals(ORDERBOOK_SIDE.BID) ? BidPrice : AskPrice;
        }

        public double[] DepthQuantity(ORDER_SIDE side)
        {
            return side.Equals(ORDER_SIDE.sell) ? BidQuantity : AskQuantity;
        }

        public double[] DepthPrice(ORDER_SIDE side)
        {
            return side.Equals(ORDER_SIDE.sell) ? BidPrice : AskPrice;
        }

        public void setDepth(ORDERBOOK_SIDE side, double price, double qty, int depth)
        {
            if (side.Equals(ORDERBOOK_SIDE.ASK))
            {
                AskQuantity[depth] = qty;
                AskPrice[depth] = price;
            }
            else if (side.Equals(ORDERBOOK_SIDE.BID))
            {
                BidQuantity[depth] = qty;
                BidPrice[depth] = price;
            }
        }

        private double[] AskQuantity { get; set; } = new double[Constants.ORDERBOOK_MAX_SIZE];

        private double[] AskPrice { get; set; } = new double[Constants.ORDERBOOK_MAX_SIZE];

        public COIN_MARKET SellMarket { get; set; }

        private double[] BidQuantity { get; set; } = new double[Constants.ORDERBOOK_MAX_SIZE];

        private double[] BidPrice { get; set; } = new double[Constants.ORDERBOOK_MAX_SIZE];

        public double PriceDiff
        {
            get { return (BidPrice[0] - AskPrice[0]); }
        }

        //public double PriceDiffApplyFee
        //{
        //    //// ToDo
        //    //get { return (BidPrice[0] * (1 - Settings.TakerFee[(int)SellMarket]) - AskPrice[0] * (1 + Settings.TakerFee[(int)BuyMarket])); }
        //    get { return -1; }
        //}

        //public double Tradable
        //{ // 호가에 있는 금액(95퍼만) , 총 잔액(95퍼만)으로 해당 호가의 가격으로 거래 가능한 갯수 중 작은 값
        //    get
        //    {
        //        return Math.Min(
        //            Math.Min(AskQuantity[0] * 0.95, (TradableTotal(ORDERBOOK_SIDE.ASK, BuyMarket) * 0.95) / AskPrice[0]),
        //            Math.Min(BidQuantity[0] * 0.95, (TradableTotal(ORDERBOOK_SIDE.BID, SellMarket) * 0.95) / BidPrice[0]));
        //    }
        //}

        //public double Tradable1
        //{ // 호가에 있는 금액(95퍼만) , 총 잔액(95퍼만)으로 해당 호가의 가격으로 거래 가능한 갯수 중 작은 값
        //    get
        //    {
        //        return Math.Min(
        //            Math.Min(AskQuantity[0] * 0.95, (Tradable_USDT[(int)ORDERBOOK_SIDE.ASK] * 0.95) / AskPrice[0]),
        //            Math.Min(BidQuantity[0] * 0.95, (Tradable_USDT[(int)ORDERBOOK_SIDE.BID] * 0.95) / BidPrice[0]));
        //    }
        //}

        //public double TradableTotal(ORDERBOOK_SIDE side, COIN_MARKET market)
        //{
        //    //// ToDo
        //    Position position = DataManager.PositionArray[(int)market, (int)COIN_TYPE.XTZ];
        //    double positionAmount = position.PositionAmmount(side.Equals(ORDERBOOK_SIDE.ASK) ? POSITION_SIDE.SHORT : POSITION_SIDE.LONG);
        //    if (market.Equals(COIN_MARKET.BINANCE))
        //    {
        //        return Tradable_USDT[(int)side] + positionAmount;
        //    }
        //    else
        //    {
        //        return Math.Max(Tradable_USDT[(int)side], positionAmount);
        //    }
        //}

        //public double Quantity
        //{
        //    get
        //    {
        //        double qty = Tradable;
        //        if ((BuyMarket.Equals(COIN_MARKET.BINANCE) && (qty * AskPrice[0] < 5))
        //            || (SellMarket.Equals(COIN_MARKET.BINANCE) && (qty * BidPrice[0] < 5)))
        //        {
        //            return 0;
        //        }
        //        //// Todo
        //        //return CommonApi.cutDecimalNumber(qty, Math.Min(Settings.DecimalLength[(int)BuyMarket], Settings.DecimalLength[(int)SellMarket]));
        //        return CommonApi.cutDecimalNumber(qty, Math.Min(4, 4));
        //    }
        //}

        //public double TotalMargin
        //{
        //    get
        //    {
        //        double qty = Quantity;

        //        //// ToDo
        //        //double tMargin = qty * BidPrice[0] * (1 - Settings.TakerFee[(int)SellMarket]) - qty * AskPrice[0] * (1 + Settings.TakerFee[(int)BuyMarket]);
        //        double tMargin = qty * BidPrice[0] * (1 - 0.001 - qty * AskPrice[0] * (1 + 0.002));
        //        return tMargin;
        //    }
        //}

        //public void Copy(Data data)
        //{
        //    for (int i = 0; i < Constants.ORDERBOOK_SIZE; i++)
        //    {
        //        AskPrice[i] = data.AskPrice[i];
        //        AskQuantity[i] = data.AskQuantity[i];
        //        BidPrice[i] = data.BidPrice[i];
        //        BidQuantity[i] = data.BidQuantity[i];
        //    }

        //    BuyMarket = data.BuyMarket;
        //    Tradable_USDT[(int)ORDERBOOK_SIDE.ASK] = data.Tradable_USDT[(int)ORDERBOOK_SIDE.ASK];

        //    SellMarket = data.SellMarket;
        //    Tradable_USDT[(int)ORDERBOOK_SIDE.BID] = data.Tradable_USDT[(int)ORDERBOOK_SIDE.BID];
        //}

        //public double BuyOrderPrice { get; set; } = 0;
        //public double SellOrderPrice { get; set; } = 0;
    }
}