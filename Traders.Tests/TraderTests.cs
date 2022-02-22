using Configuration;
using DataModels;
using Markets;
using Markets.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Traders.Tests.Mocks;

namespace Traders.Tests
{
    [TestClass()]
    public class TraderTests
    {
        [TestMethod()]
        public void SetGetPositionTest()
        {
            IMarket mockMarket = new Market(COIN_MARKET.BINANCE, new Settings(), new MockRequestControl(new MockRequestFactory()), new MockResponseControl());
            Position pos = new Position(COIN_MARKET.BINANCE, COIN_TYPE.BTC);
            Trader trader = new Trader();

            trader.RegisterMarket(mockMarket);
            trader.SetPosition(pos.Market, pos.CoinType, pos);

            Assert.IsTrue(trader.GetPosition(COIN_MARKET.BINANCE, COIN_TYPE.BTC).Market.Equals(COIN_MARKET.BINANCE));
            Assert.IsTrue(trader.GetPosition(COIN_MARKET.BINANCE, COIN_TYPE.BTC).CoinType.Equals(COIN_TYPE.BTC));
        }

        [TestMethod()]
        public void GetAllPositionTest()
        {
            Trader trader = new Trader();

            foreach (COIN_MARKET market in Enum.GetValues(typeof(COIN_MARKET)))
            {
                trader.RegisterMarket(
                    new Market(market, new Settings(), new MockRequestControl(new MockRequestFactory()), new MockResponseControl()));
            }

            foreach (COIN_MARKET market in Enum.GetValues(typeof(COIN_MARKET)))
            {
                foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
                {
                    trader.SetPosition(market, coinType, new Position(market, coinType));
                }
            }

            IList<Position> allCoin = trader.GetAllPosition();

            Assert.IsTrue(allCoin.Count.Equals(Enum.GetValues(typeof(COIN_MARKET)).Length * Enum.GetValues(typeof(COIN_TYPE)).Length));
        }

        [TestMethod()]
        public void SetGetBalanceTest()
        {
            Trader trader = new Trader();

            Balance balance = new Balance();

            foreach (COIN_MARKET market in Enum.GetValues(typeof(COIN_MARKET)))
            {
                trader.SetBalance(market, new Balance() { Market = market });
            }

            foreach (COIN_MARKET market in Enum.GetValues(typeof(COIN_MARKET)))
            {
                Assert.IsNotNull(trader.GetBalance(market));
            }
        }
    }
}