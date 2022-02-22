namespace Communication
{
    using Communication.Interfaces;
    using Configuration;
    using DataModels;
    using DataModels.Exceptions;
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class WebSocketStreamCommunicator : CommunicatorBase, ICommunicator, IJobPublisher
    {
        private ClientWebSocket webSocket = null;

        private readonly int bufferSize = 409600;

        private byte[] buffer = null;

        private bool isBusy = false;

        private ConcurrentQueue<string> jobQueue;

        public DATA_SOURCE COMMUNICATOR_TYPE => DATA_SOURCE.WEBSOCKET;

        public bool IsConnected { get => this.webSocket != null && this.webSocket.State != WebSocketState.Open; }

        private REQUEST_TYPE myReqType;

        public WebSocketStreamCommunicator()
        {
            this.buffer = new byte[bufferSize];
        }

        public AutoResetEvent Reqeust(IRequest request)
        {
            AutoResetEvent doneEvent = new AutoResetEvent(false);

            try
            {
                this.isBusy = true;

                while (this.webSocket == null)
                {
                    Thread.Sleep(100);
                }

                if (this.webSocket.State.Equals(WebSocketState.Open).Equals(false))
                {
                    throw new WebSocketException();
                }

                byte[] byteMsg = Encoding.UTF8.GetBytes(request.GetResponse());
                ArraySegment<byte> segment = new ArraySegment<byte>(byteMsg, 0, byteMsg.Length);

                Task.WaitAll(this.webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None));
            }
            catch (WebSocketException e)
            {
                APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET, new ApiCallException(REQUEST_STATE.INVAILD_SOCKET, e));
                result.DoneEvent = doneEvent;
                this.Notify(result);
            }
            catch (IOException io)
            {
                APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET, new ApiCallException(REQUEST_STATE.CHUNK_INVAILD, io));
                result.DoneEvent = doneEvent;
                this.Notify(result);
            }
            catch (NullReferenceException e)
            {
                APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET, new ApiCallException(REQUEST_STATE.EMPTY, e));
                result.DoneEvent = doneEvent;
                this.Notify(result);
            }
            catch (Exception e)
            {
                APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET, new ApiCallException(REQUEST_STATE.UNKNOWN, e));
                result.DoneEvent = doneEvent;
                this.Notify(result);
            }

            return doneEvent;
        }

        /// <summary>
        /// Connects the server.
        /// </summary>
        /// <param name="uriString">The URI string.</param>
        private void ConnectServer(string uriString, REQUEST_TYPE reqType)
        {
            int retryCount = 3;

            for (int cnt = 0; cnt < retryCount; ++cnt)
            {
                Task<ClientWebSocket> sendCreateSocketTask = Task.Run(() => this.CreateWebSocket(uriString));
                sendCreateSocketTask.Wait();

                if (sendCreateSocketTask.Result != null)
                {
                    while (this.webSocket != null)
                    {
                        Thread.Sleep(100);
                    }

                    this.webSocket = sendCreateSocketTask.Result;
                    this.myReqType = reqType;
                    break;
                }
            }

            return;
        }

        /// <summary>
        /// Disconnects the server.
        /// </summary>
        private void DisconnectServer()
        {
            Task task = Task.Run(() => this.CloseWebSocket());
            task.Wait();

            GC.Collect();

            return;
        }

        /// <summary>
        /// Creates the web socket.
        /// </summary>
        /// <param name="uriString">The URI string.</param>
        /// <returns></returns>
        protected async Task<ClientWebSocket> CreateWebSocket(string uriString)
        {
            ClientWebSocket ws = new ClientWebSocket();

            Uri uri = new Uri(uriString);
            try
            {
                await ws.ConnectAsync(uri, CancellationToken.None);

                while (!ws.State.Equals(WebSocketState.Open))
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return ws;
        }

        /// <summary>
        /// Closes the web socket.
        /// </summary>
        protected async void CloseWebSocket()
        {
            if (this.webSocket == null)
            {
                return;
            }

            if (!this.webSocket.State.Equals(WebSocketState.Aborted))
            {
                //await this.webSocket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, "Force disconnected", CancellationToken.None);

                await this.webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Force disconnected", CancellationToken.None);

                while (this.webSocket != null && !this.webSocket.State.Equals(WebSocketState.Closed))
                {
                    Thread.Sleep(10);
                }
            }

            this.webSocket = null;
        }

        /// <summary>
        /// Starts the receive job.
        /// </summary>
        /// <param name="jobQueue">The job queue.</param>
        private void StartReceiveJob(ConcurrentQueue<string> jobQueue)
        {
            this.jobQueue = jobQueue;
            this.Start();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            Thread receiveThread = new Thread(this.ReceiveOrderBook);
            receiveThread.Start();
        }

        /// <summary>
        /// Receives the order book.
        /// </summary>
        private async void ReceiveOrderBook()
        {
            int count = 0;
            WebSocketReceiveResult receiveMsg;
            ArraySegment<byte> resultSegment;

            while (true)
            {
                try
                {
                    while (this.webSocket == null ||
                        this.webSocket.State.Equals(WebSocketState.Aborted) ||
                        this.webSocket.State.Equals(WebSocketState.Closed))
                    {
                        Thread.Sleep(1000);
                    }

                    Array.Clear(this.buffer, '\0', this.buffer.Length);
                    resultSegment = new ArraySegment<byte>(buffer, 0, buffer.Length);

                    receiveMsg = await this.webSocket.ReceiveAsync(resultSegment, CancellationToken.None);

                    count = receiveMsg.Count;
                    while (!receiveMsg.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            this.DisconnectServer();
                            return;
                        }

                        resultSegment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        receiveMsg = await this.webSocket.ReceiveAsync(resultSegment, CancellationToken.None);
                        count += receiveMsg.Count;
                    }

                    string resultStrMsg = Encoding.UTF8.GetString(buffer, 0, count);

                    APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET);
                    result.Result = resultStrMsg;
                    result.Method = this.myReqType;
                    result.DoneEvent = new AutoResetEvent(false);

                    this.Notify(result);
                }
                catch (WebSocketException e)
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET, new ApiCallException(REQUEST_STATE.INVAILD_SOCKET, e));
                    result.DoneEvent = new AutoResetEvent(false);
                    this.Notify(result);

                    this.DisconnectServer();
                }
                catch (IOException io)
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET, new ApiCallException(REQUEST_STATE.CHUNK_INVAILD, io));
                    result.DoneEvent = new AutoResetEvent(false);
                    this.Notify(result);

                    this.DisconnectServer();
                }
                catch (NullReferenceException e)
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET, new ApiCallException(REQUEST_STATE.EMPTY, e));
                    result.DoneEvent = new AutoResetEvent(false);
                    this.Notify(result);

                    this.DisconnectServer();
                }
                catch (Exception e)
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    APIResult result = APIResult.Create(DATA_SOURCE.WEBSOCKET, new ApiCallException(REQUEST_STATE.INVAILD_SOCKET, e));
                    result.DoneEvent = new AutoResetEvent(false);
                    this.Notify(result);

                    this.DisconnectServer();
                }
                finally
                {
                    this.isBusy = false;
                }
            }
        }
    }
}