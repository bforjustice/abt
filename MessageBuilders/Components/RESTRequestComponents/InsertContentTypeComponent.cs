namespace MessageBuilders.Components.RESTRequestComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using RestSharp;
    using System;
    using System.Net;

    public class InsertContentTypeComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        private string myContentValue = string.Empty;

        public override object Result => throw new NotImplementedException();

        public InsertContentTypeComponent(string key, string value)
        {
            this.myKey = key;
            this.myContentValue = value;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            httpWebRequest.ContentType = this.myContentValue;
            return httpWebRequest;
        }

        public HttpWebRequest Create()
        {
            throw new NotImplementedException();
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            restRequest.AddHeader("Content-Type", this.myContentValue);
            return restRequest;
        }
    }
}