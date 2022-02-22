namespace DataModels
{
    using Configuration;
    using DataModels.Exceptions;
    using System;
    using System.Collections.Generic;

    public class Position : ItemBase
    {
        private double myTotalPosition = double.MaxValue;

        public static Position Empty()
        {
            Position position = new Position(COIN_MARKET.BINANCE, COIN_TYPE.BTC);
            position.STATE = REQUEST_STATE.EMPTY;

            return position;
        }

        public static Position Create()
        {
            Position position = new Position(COIN_MARKET.BINANCE, COIN_TYPE.BTC);
            position.STATE = REQUEST_STATE.NORMAL;

            return position;
        }

        public static Position Create(ApiCallException e)
        {
            Position position = new Position(COIN_MARKET.BINANCE, COIN_TYPE.BTC);
            position.STATE = e.STATE;

            return position;
        }

        public static Position Create(Exception e)
        {
            Position position = new Position(COIN_MARKET.BINANCE, COIN_TYPE.BTC);
            position.STATE = REQUEST_STATE.UNKNOWN;

            return position;
        }

        public REQUEST_STATE STATE { get; private set; }

        public Position(COIN_MARKET coinMarket, COIN_TYPE type)
        {
            Market = coinMarket;
            CoinType = type;

            this.myQuantity = new Dictionary<POSITION_SIDE, double>()
                                { { POSITION_SIDE.SHORT, double.MaxValue }, {POSITION_SIDE.LONG, double.MaxValue } };
            this.myPrice = new Dictionary<POSITION_SIDE, double>()
                                { { POSITION_SIDE.SHORT, double.MaxValue }, {POSITION_SIDE.LONG, double.MaxValue } };
            this.myPNL = new Dictionary<POSITION_SIDE, double>()
                                { { POSITION_SIDE.SHORT, double.MaxValue }, {POSITION_SIDE.LONG, double.MaxValue } };
            this.myLeverage = new Dictionary<POSITION_SIDE, double>()
                                { { POSITION_SIDE.SHORT, double.MaxValue }, {POSITION_SIDE.LONG, double.MaxValue } };
        }

        public COIN_TYPE CoinType { get; set; }

        private IDictionary<POSITION_SIDE, double> myQuantity;
        private IDictionary<POSITION_SIDE, double> myPrice;
        private IDictionary<POSITION_SIDE, double> myPNL;
        private IDictionary<POSITION_SIDE, double> myLeverage;

        //public double[] Quantity { get; set; } = new double[Constants.POSITION_COUNT];
        //public double[] Price { get; set; } = new double[Constants.POSITION_COUNT];
        //public double[] PNL { get; set; } = new double[Constants.POSITION_COUNT];
        //public double[] Leverage { get; set; } = new double[Constants.POSITION_COUNT];

        public double Total
        {
            get
            {
                return this.myTotalPosition;
            }
            set
            {
                this.myTotalPosition = value;
            }
        }

        public double GetQuantity(POSITION_SIDE side)
        {
            return this.myQuantity[side];
        }

        public void SetQuantity(POSITION_SIDE side, double value)
        {
            this.myQuantity[side] = value;
        }

        public double GetPrice(POSITION_SIDE side)
        {
            return this.myPrice[side];
        }

        public void SetPrice(POSITION_SIDE side, double value)
        {
            this.myPrice[side] = value;
        }

        public double GetPNL(POSITION_SIDE side)
        {
            return this.myPNL[side];
        }

        public void SetPNL(POSITION_SIDE side, double value)
        {
            this.myPNL[side] = value;
        }

        public void SetLeverage(POSITION_SIDE side, double value)
        {
            this.myLeverage[side] = value;
        }

        public double GetLeverage(POSITION_SIDE side)
        {
            return this.myLeverage[side];
        }

        public double GetLeverage()
        {
            return Math.Max(this.myLeverage[POSITION_SIDE.SHORT], this.myLeverage[POSITION_SIDE.LONG]);
        }

        public double PositionAmount(POSITION_SIDE side)
        {
            if (side.Equals(POSITION_SIDE.LONG))
            {
                return GetPrice(POSITION_SIDE.LONG) * GetQuantity(POSITION_SIDE.LONG);
            }
            else
            {
                return GetPrice(POSITION_SIDE.SHORT) * GetQuantity(POSITION_SIDE.SHORT);
            }
        }
    }
}