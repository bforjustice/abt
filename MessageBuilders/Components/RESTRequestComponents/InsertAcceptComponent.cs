namespace MessageBuilders.Components.RESTRequestComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using RestSharp;
    using System;
    using System.Net;

    public class InsertAcceptComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        private string myAcceptValue = string.Empty;

        public override object Result => throw new NotImplementedException();

        public InsertAcceptComponent(string key, string value)
        {
            this.myKey = key;
            this.myAcceptValue = value;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            httpWebRequest.Accept = this.myAcceptValue;
            return httpWebRequest;
        }

        public HttpWebRequest Create()
        {
            throw new NotImplementedException();
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            restRequest.AddHeader("Accept", this.myAcceptValue);
            return restRequest;
        }
    }
}