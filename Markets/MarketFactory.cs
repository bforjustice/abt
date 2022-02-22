namespace Markets
{
    using Communication;
    using Configuration;
    using DataModels;
    using Markets.Controls;
    using Markets.Controls.RequestControls;
    using Markets.Controls.ResponseControls;
    using Markets.Factories;
    using Markets.Interfaces;
    using System;
    using System.Collections.Generic;

    public static class MarketFactory
    {
        private static IDictionary<COIN_MARKET, string> myConfigPath =
            new Dictionary<COIN_MARKET, string>()
            {
                {COIN_MARKET.BINANCE, "..//..//..//ConfigurationFiles//Markets//RequestSpec_Binance.json"},
                {COIN_MARKET.HUOBI, "..//..//..//ConfigurationFiles//Markets//RequestSpec_Huobi.json"},
                {COIN_MARKET.OKEX, "..//..//..//ConfigurationFiles//Markets//RequestSpec_OKEX.json"},
                {COIN_MARKET.BYBIT, "..//..//..//ConfigurationFiles//Markets//RequestSpec_Bybit.json"},
                {COIN_MARKET.FTX, "..//..//..//ConfigurationFiles//Markets//RequestSpec_FTX.json"},
                {COIN_MARKET.BITGET, "..//..//..//ConfigurationFiles//Markets//BitgetAPIDatas.json"},
                {COIN_MARKET.BITZ, "..//..//..//ConfigurationFiles//Markets//RequestSpec_BitZ.json"},
                {COIN_MARKET.MXC, "..//..//..//ConfigurationFiles//Markets//RequestSpec_MXC.json"},
                {COIN_MARKET.ZBG, "..//..//..//ConfigurationFiles//Markets//ZBGAPIDatas.json"},
                {COIN_MARKET.GATEIO, "..//..//..//ConfigurationFiles//Markets//RequestSpec_GATEIO.json"},
            };

        public static IMarket CreateEmptyMarket()
        {
            IMarket market = new Market(COIN_MARKET.BINANCE,
                new Settings(),
                new EmptyRequestControl(null),
                new EmptyResponseControl());

            market.SetMarketState(MARKET_STATE.EMPTY);
            return market;
        }

        public static IMarket CreateMarket(COIN_MARKET marketType, Settings settings)
        {
            IMarket market;
            IList<DateTime> stopTimes = new List<DateTime>();

            RequestControlBase requester;
            ResponseControlBase responser;

            switch (marketType)
            {
                case COIN_MARKET.BINANCE:
                    {
                        requester = new BinanceRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.BINANCE]));
                        responser = new BinanceResponseControl();

                        market = new Market(COIN_MARKET.BINANCE,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(new RESTAPICommunicator());
                        market.POSITION_TYPE = POSITION_TYPE.ONEWAY;

                        market.GMT = 0;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        market.RegisterOrderStopTime(stopTimes);

                        return market;
                    }

                case COIN_MARKET.HUOBI:
                    {
                        requester = new HuobiRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.HUOBI]));
                        responser = new HuobiResponseControl();

                        market = new Market(COIN_MARKET.HUOBI,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(new RESTAPICommunicator());

                        market.GMT = 8;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        market.RegisterOrderStopTime(stopTimes);

                        return market;
                    }

                case COIN_MARKET.BYBIT:
                    {
                        requester = new BybitRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.BYBIT]));
                        responser = new BybitResponseControl();

                        market = new Market(COIN_MARKET.BYBIT,
                                settings,
                                requester,
                                responser);

                        market.RegisterCommuniator(new RESTAPICommunicator());

                        market.GMT = 0;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        return market;
                    }

                case COIN_MARKET.BITGET:
                    {
                        requester = new BitgetRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.BITGET]));
                        responser = new BitgetResponseControl();

                        market = new Market(COIN_MARKET.BITGET,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(new RESTAPICommunicator());

                        market.GMT = 8;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        return market;
                    }

                case COIN_MARKET.OKEX:
                    {
                        requester = new OKExRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.OKEX]));
                        responser = new OKExResponseControl();

                        market = new Market(COIN_MARKET.OKEX,
                            settings,
                            requester,
                            responser);

                        market.RegisterCommuniator(new RESTAPICommunicator());

                        market.GMT = 8;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        market.RegisterOrderStopTime(stopTimes);

                        return market;
                    }

                case COIN_MARKET.FTX:
                    {
                        requester = new FTXRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.FTX]));
                        responser = new FTXResponseControl();

                        market = new Market(COIN_MARKET.FTX,
                            settings,
                            requester,
                            responser);

                        market.GMT = 8;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        market.RegisterOrderStopTime(stopTimes);
                        market.RegisterCommuniator(new RESTAPICommunicator());

                        return market;
                    }

                case COIN_MARKET.BITZ:
                    {
                        requester = new BitZRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.BITZ]));
                        responser = new BitZResponseControl();

                        market = new Market(COIN_MARKET.BITZ,
                            settings,
                            requester,
                            responser);

                        market.GMT = 8;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        market.RegisterOrderStopTime(stopTimes);
                        market.RegisterCommuniator(new RESTAPICommunicator());

                        return market;
                    }

                case COIN_MARKET.MXC:
                    {
                        requester = new MXCRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.MXC]));
                        responser = new MXCResponseControl();

                        market = new Market(COIN_MARKET.MXC,
                            settings,
                            requester,
                            responser);

                        market.GMT = 8;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        market.RegisterOrderStopTime(stopTimes);
                        market.RegisterCommuniator(new RESTAPICommunicator());

                        return market;
                    }

                case COIN_MARKET.ZBG:
                    {
                        requester = new ZBGRequestControl(new RequestFactory(myConfigPath[COIN_MARKET.ZBG]));
                        responser = new ZBGResponseControl();

                        market = new Market(COIN_MARKET.ZBG,
                            settings,
                            requester,
                            responser);

                        market.GMT = 8;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        market.RegisterOrderStopTime(stopTimes);
                        market.RegisterCommuniator(new RESTAPICommunicator());

                        return market;
                    }

                case COIN_MARKET.GATEIO:
                    {
                        requester = new GateIORequestControl(new RequestFactory(myConfigPath[COIN_MARKET.GATEIO]));
                        responser = new GateIOResponseControl();

                        market = new Market(COIN_MARKET.GATEIO,
                            settings,
                            requester,
                            responser);

                        market.GMT = 8;
                        stopTimes.Add(new DateTime(1970, 1, 1, 0, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 8, 0, 0));
                        stopTimes.Add(new DateTime(1970, 1, 1, 16, 0, 0));

                        market.RegisterOrderStopTime(stopTimes);
                        market.RegisterCommuniator(new RESTAPICommunicator());

                        return market;
                    }
                default:
                    return null;
            }
        }
    }
}