namespace Markets.Tests.Mocks
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class MockOrderBookApiBase : ICommunicator, IJobPublisher
    {
        private IList<IJobSubscriber> communicationPeers;

        public DATA_SOURCE COMMUNICATOR_TYPE => DATA_SOURCE.REST;

        public bool IsConnected { get => false; }

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
            return new AutoResetEvent(true);
        }

        public AutoResetEvent DiscoonectStream()
        {
            throw new System.NotImplementedException();
        }

        public string LoadFile(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                return file.ReadToEnd();
            }
        }
    }

    public class MockBinanceOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BinanceOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BinanceOrder.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BinanceOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BinanceCancel.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BinanceTickers.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockBITZOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BITZOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BITZOrder.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BITZOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BITZCancel.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BITZTickers.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockHuobiOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\HuobiOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\HuobiOrder.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\HuobiOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\HuobiCancel.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\HuobiTickers.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockOKExOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\OKExOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\OKExOrder.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\OKExOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\OKExCancel.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\OKExTickers.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockZBGOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\ZBGOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\ZBGOrder.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\ZBGOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\ZBGCancel.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\ZBGTickers.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockBybitOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BybitOrderbook.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BybitOrder.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BybitOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BybitCancel.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BybitTickers.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockMXCOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\MXCOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\MXCOrder.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\MXCOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\MXCCancel.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\MXCTickers.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockBitgetOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BitgetOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BitgetOrder.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BitgetOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BitgetCancel.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\BitgetTickers.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockFTXOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\FTXOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\FTXOrder.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\FTXOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\FTXCancel.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\FTXTickers.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

            return done;
        }
    }

    public class MockGateIOOrderBookApiCall : MockOrderBookApiBase
    {
        public override AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);

            switch (request.RequestType)
            {
                case REQUEST_TYPE.ORDERBOOK:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\GateIOOrderbook.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.PLACE_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\GateIOOrder.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.OPEN_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Identifier = request.Identifier();
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\GateIOOrderInfo.json").ToString();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.CANCEL_ORDER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\GateIOCancel.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                case REQUEST_TYPE.GET_TICKER:
                    Task.Factory.StartNew(() =>
                    {
                        APIResult res = APIResult.Create(DATA_SOURCE.REST);
                        res.Method = request.RequestType;
                        res.Result = this.LoadFile(@"..\..\..\Markets.Tests\MockDatas\GateIOTickers.json").ToString();
                        res.Identifier = request.Identifier();
                        res.DoneEvent = done;

                        this.Notify(res);
                    });
                    break;

                default:
                    break;
            }

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