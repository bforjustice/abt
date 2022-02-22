using Configuration;
using DataModels;
using Markets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Traders;
using Traders.Interfaces;

namespace CalculationEngine.Algorithm.Tests
{
    [TestClass()]
    public class AppliedAREEPAlgTests
    {
        [TestMethod()]
        public void SelectBuyOpenPositionWithPositionValueZeroTest()
        {
            ITrader trader = this.GetDefaultTrader();

            // TestMarket : Bianance
            Order order = new Order();
            order.Market = COIN_MARKET.HUOBI;
            order.Symbol = "BTC-USDT";
            order.Side = ORDER_SIDE.buy;

            Order newOrder = AlgManager.Instance.AppliedAREEPAlg(trader, order);

            Assert.IsTrue(newOrder.Direction.Equals(ORDER_DIRECTION.OPEN));
        }

        [TestMethod()]
        public void SelectSellOpenPositionWithPositionValueZeroTest()
        {
            ITrader trader = this.GetDefaultTrader();

            // TestMarket : Bianance
            Order order = new Order();
            order.Market = COIN_MARKET.HUOBI;
            order.Symbol = "BTC-USDT";
            order.Side = ORDER_SIDE.sell;

            Order newOrder = AlgManager.Instance.AppliedAREEPAlg(trader, order);

            Assert.IsTrue(newOrder.Direction.Equals(ORDER_DIRECTION.OPEN));
        }

        [TestMethod()]
        public void SelectBuyOpenPositionWithExistingPositionValueTest()
        {
            ITrader trader = this.GetDefaultTrader();

            // TestMarket : Bianance
            COIN_TYPE coinType = COIN_TYPE.BTC;

            Order order = new Order();
            order.Market = COIN_MARKET.BINANCE;
            order.Symbol = "BTCUSDT";
            order.Side = ORDER_SIDE.buy;
            order.OrderPrice = 10;
            order.Quantity = 20;

            Position pos = new Position(order.Market, coinType);
            pos.SetLeverage(POSITION_SIDE.LONG, 20);
            pos.SetPNL(POSITION_SIDE.LONG, 1);
            // Total Amount 400
            pos.SetPrice(POSITION_SIDE.LONG, 20);
            pos.SetQuantity(POSITION_SIDE.LONG, 20);
            trader.SetPosition(order.Market, COIN_TYPE.BTC, pos);

            Order newOrder = AlgManager.Instance.AppliedAREEPAlg(trader, order);

            Assert.IsTrue(newOrder.Direction.Equals(ORDER_DIRECTION.OPEN));
        }

        [TestMethod()]
        public void SelectSellOpenPositionWithExistingPositionValueTest()
        {
            ITrader trader = this.GetDefaultTrader();

            // TestMarket : Bianance
            COIN_TYPE coinType = COIN_TYPE.BTC;

            Order order = new Order();
            order.Market = COIN_MARKET.HUOBI;
            order.Symbol = "BTC-USDT";
            order.Side = ORDER_SIDE.sell;
            order.OrderPrice = 19;
            order.Quantity = 20;

            Position pos = new Position(order.Market, coinType);
            pos.SetLeverage(POSITION_SIDE.SHORT, 20);
            pos.SetPNL(POSITION_SIDE.SHORT, 1);
            // Total Amount 400
            pos.SetPrice(POSITION_SIDE.SHORT, 20);
            pos.SetQuantity(POSITION_SIDE.SHORT, 20);
            trader.SetPosition(order.Market, COIN_TYPE.BTC, pos);

            Order newOrder = AlgManager.Instance.AppliedAREEPAlg(trader, order);

            Assert.IsTrue(newOrder.Direction.Equals(ORDER_DIRECTION.CLOSE));
            Assert.IsTrue(newOrder.Side.Equals(ORDER_SIDE.sell));
        }

        [TestMethod()]
        public void SelectBuyClosePositionWithExistingPositionValueTest()
        {
            ITrader trader = this.GetDefaultTrader();

            // TestMarket : Bianance
            COIN_TYPE coinType = COIN_TYPE.BTC;

            Order order = new Order();
            order.Market = COIN_MARKET.HUOBI;
            order.Symbol = "BTC-USDT";
            order.Side = ORDER_SIDE.buy;
            order.OrderPrice = 10;
            order.Quantity = 20;

            Position pos = new Position(order.Market, coinType);
            pos.SetLeverage(POSITION_SIDE.LONG, 20);
            pos.SetPNL(POSITION_SIDE.LONG, 1);
            // Total Amount 400
            pos.SetPrice(POSITION_SIDE.LONG, 20);
            pos.SetQuantity(POSITION_SIDE.LONG, 20);
            trader.SetPosition(order.Market, COIN_TYPE.BTC, pos);

            Order newOrder = AlgManager.Instance.AppliedAREEPAlg(trader, order);

            Assert.IsTrue(newOrder.Direction.Equals(ORDER_DIRECTION.CLOSE));
        }

        [TestMethod()]
        public void SelectSellClosePositionWithExistingPositionValueTest()
        {
            ITrader trader = this.GetDefaultTrader();

            // TestMarket : Bianance
            COIN_TYPE coinType = COIN_TYPE.BTC;

            Order order = new Order();
            order.Market = COIN_MARKET.HUOBI;
            order.Symbol = "BTC-USDT";
            order.Side = ORDER_SIDE.sell;
            order.OrderPrice = 10;
            order.Quantity = 20;

            Position pos = new Position(order.Market, coinType);
            pos.SetLeverage(POSITION_SIDE.SHORT, 20);
            pos.SetPNL(POSITION_SIDE.SHORT, 1);
            // Total Amount 400
            pos.SetPrice(POSITION_SIDE.SHORT, 20);
            pos.SetQuantity(POSITION_SIDE.SHORT, 20);
            trader.SetPosition(order.Market, COIN_TYPE.BTC, pos);

            Order newOrder = AlgManager.Instance.AppliedAREEPAlg(trader, order);

            Assert.IsTrue(newOrder.Direction.Equals(ORDER_DIRECTION.CLOSE));
        }

        private void SetPositionValue(ITrader trader,
                COIN_MARKET coinMarket,
                COIN_TYPE coinType,
                POSITION_SIDE side,
                double leverage,
                double price,
                double PNL,
                double value)
        {
            Position pos = new Position(coinMarket, coinType);

            pos.SetLeverage(side, leverage);
            pos.SetPrice(side, price);
            pos.SetPNL(side, PNL);
            pos.SetQuantity(side, value);

            trader.SetPosition(coinMarket, coinType, pos);
        }

        private ITrader GetDefaultTrader()
        {
            ITrader trader = new Trader();
            IList<Position> posList = new List<Position>();
            foreach (COIN_MARKET coinMarket in Enum.GetValues(typeof(COIN_MARKET)))
            {
                trader.RegisterMarket(MarketFactory.CreateMarket(coinMarket, new Settings()));

                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    Position pos = new Position(coinMarket, coinType);

                    pos.SetPrice(POSITION_SIDE.LONG, 0);
                    pos.SetQuantity(POSITION_SIDE.LONG, 0);
                    pos.SetPNL(POSITION_SIDE.LONG, 0);
                    pos.SetLeverage(POSITION_SIDE.LONG, 0);

                    pos.SetPrice(POSITION_SIDE.SHORT, 0);
                    pos.SetQuantity(POSITION_SIDE.SHORT, 0);
                    pos.SetPNL(POSITION_SIDE.SHORT, 0);
                    pos.SetLeverage(POSITION_SIDE.SHORT, 0);

                    pos.Total = 0;

                    trader.SetPosition(coinMarket, coinType, pos);
                }
            }

            return trader;
        }
    }
}