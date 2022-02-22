namespace MessageBuilders
{
    using LogTrace.Interfaces;
    using MessageBuilders.Interfaces;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MessageBuilders;
    using System.Text;
    using System.Threading.Tasks;
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using MessageBuilders.Components.Interfaces;
    using MessageBuilders.Components.RESTBuilder.OptionModels;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.IO;
    using RestSharp.Deserializers;

    public class RESTRequester : IRequestCreator, IGeneralRestRequest
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CommLogger");

        private IList<IRESTRequestComponentCreator> workflow;

        private IDictionary<string, IBlockComponent<object>> components;

        private IDictionary<string, IParamWorker<RequestOptions>> myWorkerComponents;

        private IRestRequest myRequest;

        private IRestClient myClient;

        public RESTRequester()
        {
            this.workflow = new List<IRESTRequestComponentCreator>();
            this.components = new Dictionary<string, IBlockComponent<object>>();
        }

        public void CreateComponent(JObject obj)
        {
            IBlockComponent<object> component = null;

            switch (obj["ValueType"].Value<string>())
            {
                case "Component":
                    {
                        component = ComponentFactory.CreateComponent(obj["Type"].Value<string>(), obj["Key"].Value<string>());

                        foreach (JValue item in obj["Value"].Values<JValue>())
                        {
                            component.SetSubComponent(this.GetComponent(item.Value<string>()));
                        }
                        break;
                    }

                case "Worker":
                    {
                        IParamWorker<RequestOptions> comp = 
                            ComponentFactory.CreateWorker(
                                obj.SelectToken("Type").Value<string>(), 
                                obj.SelectToken("Key").Value<string>()
                                );

                        foreach (JValue item in obj["Value"].Values<JValue>())
                        {
                            comp.SetComponent(this.GetComponent(item.Value<string>()));
                        }

                        this.myWorkerComponents.Add(comp.Key, comp);

                        return;
                    }

                case "Parameter":
                    JToken paramType;
                    string typeValue = "String";
                    if (obj.TryGetValue("ParameterType", StringComparison.CurrentCulture, out paramType))
                    {
                        typeValue = paramType.Value<string>();
                    }

                    component = ComponentFactory.CreateParameterComponent(obj["Key"].Value<string>(), typeValue);
                    break;

                case "Value":
                    component = ComponentFactory.CreateValueComponent(
                        obj["Type"].Value<string>(), 
                        obj["Key"].Value<string>(), 
                        obj["Value"].FirstOrDefault().Value<string>());
                    break;

                default:
                    throw new NotSupportedException("Not support type");
            }

            this.components.Add(component.Key, component);
        }

        public void CreateWorkflow(IEnumerable<JToken> jVal)
        {
            foreach (JValue val in jVal)
            {
                this.workflow.Add(this.GetComponent(val.Value<string>()) as IRESTRequestComponentCreator);
            }
        }

        public IGeneralRestRequest Create(IDictionary<string, string> parameters)
        {
            try
            {
                foreach (string key in parameters.Keys)
                {
                    (this.GetComponent(key) as IParameterComponent<object>).SetValue(parameters[key]);
                }

                foreach (IRESTRequestComponentCreator creator in this.workflow)
                {
                    this.myRequest = creator.Do(this.myRequest);
                }

                this.myClient = new RestClient(this.GetComponent("BaseUrl").Result.ToString());
                this.myClient.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

                return this;
            }
            catch (Exception e)
            {
                myLogger.Error($"Error Create Reqeuster : {e.StackTrace}, {e.ToString()}");
                return null;
            }
        }

        private IBlockComponent<object> GetComponent(string key)
        {
            IBlockComponent<object> result = null;
            if (this.components.TryGetValue(key, out result))
            {
                return result;
            }

            throw new ArgumentException("Not Cotains component");
        }

        public string GetResponse()
        {
            try
            {
                this.myClient.ClearHandlers();
                if (this.myRequest.JsonSerializer is IDeserializer existingDeserializer)
                {
                    this.myClient.AddHandler("application/json", () => existingDeserializer);
                    this.myClient.AddHandler("text/json", () => existingDeserializer);
                    this.myClient.AddHandler("text/x-json", () => existingDeserializer);
                    this.myClient.AddHandler("text/html", () => existingDeserializer);
                    this.myClient.AddHandler("text/javascript", () => existingDeserializer);
                    this.myClient.AddHandler("*+json", () => existingDeserializer);
                    this.myClient.AddHandler("text/plain", () => existingDeserializer);
                }
                else
                {
                    throw new NotImplementedException();
                }

                var xmlDeserializer = new XmlDeserializer();
                this.myClient.AddHandler("application/xml", () => xmlDeserializer);
                this.myClient.AddHandler("text/xml", () => xmlDeserializer);
                this.myClient.AddHandler("*+xml", () => xmlDeserializer);
                this.myClient.AddHandler("*", () => xmlDeserializer);

                Task<IRestResponse<JToken>> response = this.ExecuteAsync(this.myRequest);

                response.Wait();

                if (!response.Result.IsSuccessful)
                {
                    myLogger.Error(this.GenResponseMsg(response.Result));
                    myLogger.Error(this.GenRequestMsg(this.myRequest as RestRequest));
                }

                this.myClient = null;
                this.myRequest = null;

                return response.Result.Content;
            }
            catch
            {
                throw;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (this.myRequest != null)
            {
                builder.Append($"URI : {((RestSharp.RestRequest)this.myRequest).Resource}\n");
            }
            else
            {
                builder.Append($"URI : Invalid URL");
            }

            return builder.ToString();
        }

        private async Task<IRestResponse<JToken>> ExecuteAsync(IRestRequest request)
        {
            try
            {
                var response = await this.myClient.ExecuteAsync<JToken>(request);
                return response;
            }
            catch
            {
                throw new InvalidOperationException();
            }
        }

        private string GenResponseMsg(IRestResponse<JToken> res)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"\nStatusCode\t\t:{res.StatusCode}\n");
            builder.Append($"AbsolutePath\t\t:{res.Request.Resource}\n");
            builder.Append($"ErrorException\t:{res.ErrorException}\n");
            builder.Append($"ErrorMessage\t\t:{res.ErrorMessage}\n");
            builder.Append($"Content\t\t:{res.Content}\n");

            return builder.ToString();
        }

        private string GenRequestMsg(IRestRequest res)
        {
            StringBuilder builder = new StringBuilder();
            if (res.Body != null)
            {
                builder.Append($"\nRequest Body \t\t:{res.Body.Value}\n");
            }

            builder.Append($"AbsolutePath\t\t:{res.Resource}\n");

            return builder.ToString();
        }
    }
}
