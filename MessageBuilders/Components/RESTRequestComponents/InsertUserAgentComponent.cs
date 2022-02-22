namespace MessageBuilders.Components.RESTRequestComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using RestSharp;
    using System;
    using System.Net;

    public class InsertUserAgentComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        private string myUserAgent = string.Empty;

        public override object Result => throw new NotImplementedException();

        public InsertUserAgentComponent(string key, string value)
        {
            this.myKey = key;
            this.myUserAgent = value;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            httpWebRequest.UserAgent = this.myUserAgent;
            return httpWebRequest;
        }

        public HttpWebRequest Create()
        {
            throw new NotImplementedException();
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            return restRequest;
        }
    }
}