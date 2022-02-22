namespace CalculationEngine.Tests.MockFactories
{
    using CalculationEngine.Tests.Mocks;
    using Configuration;
    using DataModels;
    using Markets;
    using Markets.Controls;
    using Markets.Controls.RequestControls;
    using Markets.Controls.ResponseControls;
    using Markets.Factories;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;

    public static class MockMarketFactory
    {
        public static IMarket CreateMarket(COIN_MARKET marketType, Settings settings, string mockApiCommName)
        {
            IMarket market;
            IList<DateTime> stopTimes = new List<DateTime>();

            RequestControlBase requester;
            ResponseControlBase responser;

            switch (marketType)
            {
                case COIN_MARKET.BINANCE:
                    {
                        requester = new BinanceRequestControl(new MockRequestFactory());
                        responser = new BinanceResponseControl();

                        market = new Market(COIN_MARKET.BINANCE,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.HUOBI:
                    {
                        requester = new HuobiRequestControl(new RequestFactory(string.Empty));
                        responser = new HuobiResponseControl();

                        market = new Market(COIN_MARKET.HUOBI,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.BYBIT:
                    {
                        requester = new BybitRequestControl(new MockRequestFactory());
                        responser = new BybitResponseControl();

                        market = new Market(COIN_MARKET.BYBIT,
                                settings,
                                requester,
                                responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.BITGET:
                    {
                        requester = new BitgetRequestControl(new MockRequestFactory());
                        responser = new BitgetResponseControl();

                        market = new Market(COIN_MARKET.BITGET,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.OKEX:
                    {
                        requester = new OKExRequestControl(new MockRequestFactory());
                        responser = new OKExResponseControl();

                        market = new Market(COIN_MARKET.OKEX,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.FTX:
                    {
                        requester = new FTXRequestControl(new MockRequestFactory());
                        responser = new FTXResponseControl();

                        market = new Market(COIN_MARKET.FTX,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.BITZ:
                    {
                        requester = new BitZRequestControl(new MockRequestFactory());
                        responser = new BitZResponseControl();

                        market = new Market(COIN_MARKET.BITZ,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.MXC:
                    {
                        requester = new MXCRequestControl(new MockRequestFactory());
                        responser = new MXCResponseControl();

                        market = new Market(COIN_MARKET.MXC,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.ZBG:
                    {
                        requester = new ZBGRequestControl(new MockRequestFactory());
                        responser = new ZBGResponseControl();

                        market = new Market(COIN_MARKET.ZBG,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }

                case COIN_MARKET.GATEIO:
                    {
                        requester = new GateIORequestControl(new MockRequestFactory());
                        responser = new GateIOResponseControl();

                        market = new Market(COIN_MARKET.GATEIO,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(MockAPICommFacotry.CreateMockAPIComm(mockApiCommName));

                        return market;
                    }
                default:
                    return null;
            }
        }
    }
}