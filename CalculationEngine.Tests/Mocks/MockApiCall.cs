namespace CalculationEngine.Tests.Mocks
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockApiWorkSequence
    {
        private IDictionary<REQUEST_TYPE, Queue<string>> myWorkSequences;

        public MockApiWorkSequence()
        {
            this.myWorkSequences = new Dictionary<REQUEST_TYPE, Queue<string>>();
        }

        public void Enqueue(REQUEST_TYPE type, string path)
        {
            if (!this.myWorkSequences.ContainsKey(type))
            {
                this.myWorkSequences.Add(type, new Queue<string>());
            }

            this.myWorkSequences[type].Enqueue(path);
        }

        public string Dequeue(REQUEST_TYPE type)
        {
            Queue<string> queue;
            if (this.myWorkSequences.TryGetValue(type, out queue))
            {
                if (queue.Count != 0)
                {
                    return queue.Dequeue();
                }
            }

            return string.Empty;
        }
    }

    public static class MockAPICommFacotry
    {
        public static MockApiCommunicator CreateMockAPIComm(string testCase)
        {
            MockApiWorkSequence sequence = new MockApiWorkSequence();
            switch (testCase)
            {
                case "BINANCE_SEQ_ORDER_INFO":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_Seq_1.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_Seq_2.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_Seq_3.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceCancel.json");

                    return new MockApiCommunicator(sequence);

                case "BINANCE_SUCCESS_CANCEL_ORDER_INFO":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_PatialSuccess_Se1.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_Calcelld_Se1.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_SuccessOnce_Seq_1.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_SuccessOnce_Seq_1.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_SuccessOnce_Seq_1.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_SuccessOnce_Seq_1.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceCancel.json");

                    return new MockApiCommunicator(sequence);

                case "BINANCE_SUCCESS_ONCE_ORDER_INFO":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo_TestCase_SuccessOnce_Seq_1.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceCancel.json");

                    return new MockApiCommunicator(sequence);

                case "BINANCE":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BinanceCancel.json");

                    return new MockApiCommunicator(sequence);

                case "BITZ":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\BitzOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BitzOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BitzOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BitzCancel.json");

                    return new MockApiCommunicator(sequence);

                case "HUOBI":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\HuobiOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\HuobiOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\HuobiOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\HuobiCancel.json");

                    return new MockApiCommunicator(sequence);

                case "OKEX":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\OKExOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\OKExOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\OKExOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\OKExCancel.json");

                    return new MockApiCommunicator(sequence);

                case "ZBG":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\ZBGOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\ZBGOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\ZBGOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\ZBGCancel.json");

                    return new MockApiCommunicator(sequence);

                case "BYBIT":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\BybitOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BybitOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BybitOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BybitCancel.json");

                    return new MockApiCommunicator(sequence);

                case "MXC":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\MXCOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\MXCOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\MXCOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\MXCCancel.json");

                    return new MockApiCommunicator(sequence);

                case "BITGET":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\BitgetOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BitgetOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BitgetOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\BitgetCancel.json");

                    return new MockApiCommunicator(sequence);

                case "FTX":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXCancel.json");

                    return new MockApiCommunicator(sequence);

                case "FTX_SUCCESS_ONCE_ORDER_INFO":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrderInfo_TestCase_SuccessOnce_Seq_1.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXCancel.json");

                    return new MockApiCommunicator(sequence);

                case "FTX_SUCCESS_CANCEL_ORDER_INFO":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrderInfo_TestCase_PatialSuccess_Se1.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrderInfo_TestCase_Calcelld_Se1.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXOrderInfo_TestCase_SuccessOnce_Seq_1.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\FTXCancel.json");

                    return new MockApiCommunicator(sequence);

                case "GATEIO":
                    sequence.Enqueue(REQUEST_TYPE.ORDERBOOK, @"..\..\..\CalculationEngine.Tests\MockDatas\GateIOOrderbook.json");
                    sequence.Enqueue(REQUEST_TYPE.PLACE_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\GateIOOrder.json");
                    sequence.Enqueue(REQUEST_TYPE.OPEN_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\GateIOOrderInfo.json");
                    sequence.Enqueue(REQUEST_TYPE.CANCEL_ORDER, @"..\..\..\CalculationEngine.Tests\MockDatas\GateIOCancel.json");

                    return new MockApiCommunicator(sequence);

                default:
                    throw new KeyNotFoundException();
            }
        }
    }

    public class MockApiCommunicator : ICommunicator, IJobPublisher
    {
        private IList<IJobSubscriber> communicationPeers;

        public DATA_SOURCE COMMUNICATOR_TYPE => DATA_SOURCE.REST;

        public bool IsConnected { get => false; }

        private MockApiWorkSequence myWorkSequences;

        public MockApiCommunicator(MockApiWorkSequence mockApiSequece)
        {
            this.myWorkSequences = mockApiSequece;
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

        public JObject LoadFile(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    return (JObject)JToken.ReadFrom(reader);
                }
            }
        }

        public AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent done = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    APIResult res = APIResult.Create(DATA_SOURCE.REST);
                    res.Method = request.RequestType;
                    res.Identifier = request.Identifier();
                    res.Result = this.LoadFile(this.myWorkSequences.Dequeue(request.RequestType)).ToString();
                    res.DoneEvent = done;

                    this.Notify(res);
                }
                catch (Exception e)
                {
                    throw new InvalidDataException();
                }
            });

            return done;
        }
    }
}