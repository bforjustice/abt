namespace MessageBuilders.Components.RESTRequestComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    public class CreatePostMessageComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        public override object Result => throw new NotImplementedException();

        public CreatePostMessageComponent(string key)
        {
            this.myKey = key;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string content = Convert.ToString(this.subComponent.First().Result);

            myLogger.Info($"Context Message : {content}");

            httpWebRequest.ContentLength = encoding.GetByteCount(content);

            using (Stream reqStream = httpWebRequest.GetRequestStream())
            {
                reqStream.Write(encoding.GetBytes(content), 0, encoding.GetByteCount(content));
            }

            return httpWebRequest;
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            restRequest.AddParameter("application/json", Convert.ToString(this.subComponent.First().Result), ParameterType.RequestBody);
            return restRequest;
        }

        private string ConvertUTF8(string value)
        {
            byte[] bytes = Encoding.Default.GetBytes(value);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}