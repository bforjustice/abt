namespace OrderBookHandler.Tests.Mocks
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class MockOrderBookApiBase : ICommunicator, IJobPublisher
    {
        private IList<IJobSubscriber> communicationPeers;

        public DATA_SOURCE COMMUNICATOR_TYPE => throw new System.NotImplementedException();

        public bool IsConnected { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public abstract AutoResetEvent Reqeust(IRequest request);

        public MockOrderBookApiBase()
        {
            this.communicationPeers = new List<IJobSubscriber>();
        }

        public void Subscribe(IJobSubscriber subscriber)
        {
            if (this.communicationPeers.Contains(subscriber))
            {
                return;
            }

            this.communicationPeers.Add(subscriber);
        }

        public void Unsubscribe(IJobSubscriber subscriber)
        {
            if (this.communicationPeers.Contains(subscriber))
            {
                this.communicationPeers.Remove(subscriber);
            }
        }

        public void Notify(APIResult result)
        {
            foreach (IJobSubscriber subscriber in this.communicationPeers)
            {
                subscriber.PublishJob(result);
            }
        }

        public AutoResetEvent ConnectStream(string uri)
        {
            throw new System.NotImplementedException();
        }

        public AutoResetEvent DiscoonectStream()
        {
            throw new System.NotImplementedException();
        }
    }

    public class MockBinanceOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                this.Notify(new APIResult()
                {
                    Method = REQUEST_TYPE.ORDERBOOK,
                    Result = "{\"lastUpdateId\": 1027024, " +
                    "\"E\": 1589436922972,\"T\": 1589436922959," +
                    "\"bids\":[" +
                    "[\"4.00000000\",\"431.00000000\"]," +
                    "[\"5.00000000\",\"432.00000000\"]," +
                    "[\"3.00000000\",\"433.00000000\"]," +
                    "[\"2.00000000\",\"434.00000000\"]," +
                    "[\"1.00000000\",\"435.00000000\"]," +
                    "[\"2.00000000\",\"436.00000000\"]," +
                    "[\"3.00000000\",\"437.00000000\"]," +
                    "]," +
                    "\"asks\":[" +
                        "[\"4.00000200\",\"12.00000000\"]," +
                        "[\"5.00000200\",\"13.00000000\"]," +
                        "[\"6.00000200\",\"14.00000000\"]," +
                        "[\"7.00000200\",\"15.00000000\"]," +
                        "[\"8.00000200\",\"16.00000000\"]," +
                        "[\"9.00000200\",\"17.00000000\"]," +
                        "[\"10.00000200\",\"18.00000000\"]," +
                        "]}"
                });
                done.Set();
            });

            return done;
        }
    }

    public class MockHuobiOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                this.Notify(new APIResult()
                {
                    Method = REQUEST_TYPE.ORDERBOOK,
                    Result = "{" +
                "\"ch\": \"market.BTC-USDT.depth.step0\"," +
                "\"status\": \"ok\"," +
                "\"tick\": {" +
                    "\"asks\": [" +
                        "[" +
                            "13084.2, " +
                            "168" +
                        "]," +
                        "[" +
                            "13085.6," +
                            "1" +
                        "]," +
                        "[" +
                            "13084.2, " +
                            "168" +
                        "]," +
                        "[" +
                            "13084.2, " +
                            "168" +
                        "]," +
                        "[" +
                            "13084.2, " +
                            "168" +
                        "]," +
                        "[" +
                            "13084.2, " +
                            "168" +
                        "]," +
                        "[" +
                            "13084.2, " +
                            "168" +
                        "]," +
                        "[" +
                            "13084.2, " +
                            "168" +
                        "]," +

                            "]," +
                            "\"bids\": [" +
                                "[" +
                                    "13084," +
                                    "38" +
                                "]," +
                                "[" +
                                    "13069.9," +
                                    "1" +
                                "]," +
                                "[" +
                                    "13084," +
                                    "38" +
                                "]," +
                                "[" +
                                    "13084," +
                                    "38" +
                                "]," +
                                "[" +
                                    "13084," +
                                    "38" +
                                "]," +
                                "[" +
                                    "13084," +
                                    "38" +
                                "]," +
                                "[" +
                                    "13084," +
                                    "38" +
                                "]," +
                            "]," +
                            "\"ch\": \"market.BTC-USDT.depth.step0\"," +
                            "\"id\": 1603694838," +
                            "\"mrid\": 131471527," +
                            "\"ts\": 1603694838167," +
                            "\"version\": 1603694838" +
                        "}," +
                        "\"ts\": 1603694838240" +
                    "}"
                });
                done.Set();
            });

            return done;
        }
    }

    public class MockOKExOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                this.Notify(new APIResult()
                {
                    Method = REQUEST_TYPE.ORDERBOOK,
                    Result = "{" +
                        "\"asks\":[" +
                            "[" +
                                "\"3968.5\"," +
                                "\"121\"," +
                                "0," +
                                "2" +
                            "]," +
                            "[" +
                                "\"3968.6\"," +
                                "\"160\"," +
                                "0," +
                                "4" +
                            "]," +
                            "[" +
                                "\"3968.5\"," +
                                "\"121\"," +
                                "0," +
                                "2" +
                            "]," +
                            "[" +
                                "\"3968.5\"," +
                                "\"121\"," +
                                "0," +
                                "2" +
                            "]," +
                            "[" +
                                "\"3968.5\"," +
                                "\"121\"," +
                                "0," +
                                "2" +
                            "]," +
                            "[" +
                                "\"3968.5\"," +
                                "\"121\"," +
                                "0," +
                                "2" +
                            "]," +
                            "[" +
                                "\"3968.5\"," +
                                "\"121\"," +
                                "0," +
                                "2" +
                            "]," +
                        "]," +
                        "\"bids\":[" +
                            "[" +
                                "\"3968.4\"," +
                                "\"179\"," +
                                "0," +
                                "4" +
                            "]," +
                            "[" +
                                "\"3968\"," +
                                "\"914\"," +
                                "0," +
                                "3" +
                            "]," +
                            "[" +
                                "\"3968.4\"," +
                                "\"179\"," +
                                "0," +
                                "4" +
                            "]," +
                            "[" +
                                "\"3968.4\"," +
                                "\"179\"," +
                                "0," +
                                "4" +
                            "]," +
                            "[" +
                                "\"3968.4\"," +
                                "\"179\"," +
                                "0," +
                                "4" +
                            "]," +
                            "[" +
                                "\"3968.4\"," +
                                "\"179\"," +
                                "0," +
                                "4" +
                            "]," +
                            "[" +
                                "\"3968.4\"," +
                                "\"179\"," +
                                "0," +
                                "4" +
                            "]," +
                        "]," +
                        "\"time\":\"2019-03-25T11:12:10.601Z\"" +
                    "}"
                });
                done.Set();
            });

            return done;
        }
    }

    public class MockBybitOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                this.Notify(new APIResult()
                {
                    Method = REQUEST_TYPE.ORDERBOOK,
                    Result = "{" +
                            "\"ret_code\": 0," +                              // return code
                            "\"ret_msg\": \"OK\"," +                            // error message
                            "\"ext_code\": \"\"," +                            // additional error code
                            "\"ext_info\": \"\"," +                            // additional error info
                            "\"result\": [" +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                // symbol
                                    "\"price\": \"9487\"," +             // price
                                    "\"size\": 336241," +        // size (in USD contracts)
                                    "\"side\": \"Buy\"" +          // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                // symbol
                                    "\"price\": \"9486\"," +             // price
                                    "\"size\": 336240," +        // size (in USD contracts)
                                    "\"side\": \"Buy\"" +          // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                // symbol
                                    "\"price\": \"9485\"," +             // price
                                    "\"size\": 336241," +        // size (in USD contracts)
                                    "\"side\": \"Buy\"" +          // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                // symbol
                                    "\"price\": \"9487\"," +             // price
                                    "\"size\": 336241," +        // size (in USD contracts)
                                    "\"side\": \"Buy\"" +          // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                // symbol
                                    "\"price\": \"9487\"," +             // price
                                    "\"size\": 336241," +        // size (in USD contracts)
                                    "\"side\": \"Buy\"" +          // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                // symbol
                                    "\"price\": \"9487\"," +             // price
                                    "\"size\": 336241," +        // size (in USD contracts)
                                    "\"side\": \"Buy\"" +          // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                // symbol
                                    "\"price\": \"9487\"," +             // price
                                    "\"size\": 336241," +        // size (in USD contracts)
                                    "\"side\": \"Buy\"" +          // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                // symbol
                                    "\"price\": \"9487\"," +             // price
                                    "\"size\": 336241," +        // size (in USD contracts)
                                    "\"side\": \"Buy\"" +          // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                 // symbol
                                    "\"price\": \"9487.5\"," +                  // price
                                    "\"size\": 522147," +                     // size (in USD contracts)
                                    "\"side\": \"Sell\"" +                     // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                 // symbol
                                    "\"price\": \"9488.5\"," +                  // price
                                    "\"size\": 522148," +                     // size (in USD contracts)
                                    "\"side\": \"Sell\"" +                     // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                 // symbol
                                    "\"price\": \"9489.5\"," +                  // price
                                    "\"size\": 522149," +                     // size (in USD contracts)
                                    "\"side\": \"Sell\"" +                     // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                 // symbol
                                    "\"price\": \"9487.5\"," +                  // price
                                    "\"size\": 522147," +                     // size (in USD contracts)
                                    "\"side\": \"Sell\"" +                     // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                 // symbol
                                    "\"price\": \"9487.5\"," +                  // price
                                    "\"size\": 522147," +                     // size (in USD contracts)
                                    "\"side\": \"Sell\"" +                     // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                 // symbol
                                    "\"price\": \"9487.5\"," +                  // price
                                    "\"size\": 522147," +                     // size (in USD contracts)
                                    "\"side\": \"Sell\"" +                     // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                 // symbol
                                    "\"price\": \"9487.5\"," +                  // price
                                    "\"size\": 522147," +                     // size (in USD contracts)
                                    "\"side\": \"Sell\"" +                     // side
                                "}," +
                                "{" +
                                    "\"symbol\": \"BTCUSD\"," +                 // symbol
                                    "\"price\": \"9487.5\"," +                  // price
                                    "\"size\": 522147," +                     // size (in USD contracts)
                                    "\"side\": \"Sell\"" +                     // side
                                "}" +
                            "]," +
                            "\"time_now\": \"1567108756.834357\"" +             // UTC timestamp
                            "}"
                });
                done.Set();
            });

            return done;
        }
    }

    //return "{" +
    //    "\"ret_code\": 0," +                              // return code
    //    "\"ret_msg\": \"OK\"," +                            // error message
    //    "\"ext_code\": \"\"," +                            // additional error code
    //    "\"ext_info\": \"\"," +                            // additional error info
    //    "\"result\": [" +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                // symbol
    //            "\"price\": \"9487\"," +             // price
    //            "\"size\": 336241," +        // size (in USD contracts)
    //            "\"side\": \"Buy\"" +          // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                // symbol
    //            "\"price\": \"9486\"," +             // price
    //            "\"size\": 336240," +        // size (in USD contracts)
    //            "\"side\": \"Buy\"" +          // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                // symbol
    //            "\"price\": \"9485\"," +             // price
    //            "\"size\": 336241," +        // size (in USD contracts)
    //            "\"side\": \"Buy\"" +          // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                // symbol
    //            "\"price\": \"9487\"," +             // price
    //            "\"size\": 336241," +        // size (in USD contracts)
    //            "\"side\": \"Buy\"" +          // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                // symbol
    //            "\"price\": \"9487\"," +             // price
    //            "\"size\": 336241," +        // size (in USD contracts)
    //            "\"side\": \"Buy\"" +          // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                // symbol
    //            "\"price\": \"9487\"," +             // price
    //            "\"size\": 336241," +        // size (in USD contracts)
    //            "\"side\": \"Buy\"" +          // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                // symbol
    //            "\"price\": \"9487\"," +             // price
    //            "\"size\": 336241," +        // size (in USD contracts)
    //            "\"side\": \"Buy\"" +          // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                // symbol
    //            "\"price\": \"9487\"," +             // price
    //            "\"size\": 336241," +        // size (in USD contracts)
    //            "\"side\": \"Buy\"" +          // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                 // symbol
    //            "\"price\": \"9487.5\"," +                  // price
    //            "\"size\": 522147," +                     // size (in USD contracts)
    //            "\"side\": \"Sell\"" +                     // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                 // symbol
    //            "\"price\": \"9488.5\"," +                  // price
    //            "\"size\": 522148," +                     // size (in USD contracts)
    //            "\"side\": \"Sell\"" +                     // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                 // symbol
    //            "\"price\": \"9489.5\"," +                  // price
    //            "\"size\": 522149," +                     // size (in USD contracts)
    //            "\"side\": \"Sell\"" +                     // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                 // symbol
    //            "\"price\": \"9487.5\"," +                  // price
    //            "\"size\": 522147," +                     // size (in USD contracts)
    //            "\"side\": \"Sell\"" +                     // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                 // symbol
    //            "\"price\": \"9487.5\"," +                  // price
    //            "\"size\": 522147," +                     // size (in USD contracts)
    //            "\"side\": \"Sell\"" +                     // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                 // symbol
    //            "\"price\": \"9487.5\"," +                  // price
    //            "\"size\": 522147," +                     // size (in USD contracts)
    //            "\"side\": \"Sell\"" +                     // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                 // symbol
    //            "\"price\": \"9487.5\"," +                  // price
    //            "\"size\": 522147," +                     // size (in USD contracts)
    //            "\"side\": \"Sell\"" +                     // side
    //        "}," +
    //        "{" +
    //            "\"symbol\": \"BTCUSD\"," +                 // symbol
    //            "\"price\": \"9487.5\"," +                  // price
    //            "\"size\": 522147," +                     // size (in USD contracts)
    //            "\"side\": \"Sell\"" +                     // side
    //        "}" +
    //    "]," +
    //    "\"time_now\": \"1567108756.834357\"" +             // UTC timestamp
    //    "}";

    public class MockBitgetOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                this.Notify(new APIResult()
                {
                    Method = REQUEST_TYPE.ORDERBOOK,
                    Result = "{\"asks\":[" +
                                "[\"8858.0\",\"19299\",2.0]," +
                                "[\"8859.0\",\"19300\",2.0]," +
                                "[\"8860.0\",\"19299\",2.0]," +
                                "[\"8861.0\",\"19299\",2.0]," +
                                "[\"8862.0\",\"19299\",2.0]," +
                                "[\"8863.0\",\"19299\",2.0]," +
                                "[\"8864.0\",\"19299\",2.0]" +
                                "]," +
                                "\"bids\":[" +
                                "[\"7466.0\",\"499\",1.0]," +
                                "[\"4995.0\",\"12500\",2.0]," +
                                "[\"4995.0\",\"12500\",2.0]," +
                                "[\"4995.0\",\"12500\",2.0]," +
                                "[\"4995.0\",\"12500\",2.0]," +
                                "[\"4995.0\",\"12500\",2.0]," +
                                "[\"4995.0\",\"12500\",2.0]" +
                                "],\"timestamp\":\"1591237821479\"}"
                });

                done.Set();
            });
            return done;
        }
    }

    public class MockApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            return new AutoResetEvent(false);
        }
    }
}