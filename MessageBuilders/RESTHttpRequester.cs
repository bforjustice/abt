namespace MessageBuilders
{
    using LogTrace.Interfaces;
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    public class RESTHttpRequester : IRequestCreator, IGeneralRestRequest
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CommLogger");

        private IList<IRESTRequestComponentCreator> workflow;

        private IDictionary<string, IBlockComponent<object>> components;

        private HttpWebRequest myRequest;

        public RESTHttpRequester()
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

        public void CreateWorkflow(JEnumerable<JToken> jVal)
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

                this.myRequest = null;
                foreach (IRESTRequestComponentCreator creator in this.workflow)
                {
                    this.myRequest = creator.Do(this.myRequest);
                }

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
                HttpWebResponse wRes = null;
                using (wRes = (HttpWebResponse)this.myRequest.GetResponse())
                {
                    Stream respPostStream = wRes.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, new UTF8Encoding(), true);

                    return readerPost.ReadToEnd();
                }
            }
            catch
            {
                throw;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"URI : {this.myRequest.RequestUri.ToString()}\n");

            return builder.ToString();
        }
    }
}