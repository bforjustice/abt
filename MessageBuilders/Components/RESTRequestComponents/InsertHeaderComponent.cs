namespace MessageBuilders.Components.RESTRequestComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using RestSharp;
    using System;
    using System.Linq;
    using System.Net;

    public class InsertHeaderComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        public override object Result => new HttpHeader(this.myKey, this.subComponent.First().Result.ToString());
        //public override object Result => this.subComponent.First().Result.ToString();

        public InsertHeaderComponent(string key)
        {
            this.myKey = key;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            httpWebRequest.Headers.Add(this.myKey, Convert.ToString(this.subComponent.First().Result));
            return httpWebRequest;
        }

        public HttpWebRequest Create()
        {
            throw new NotImplementedException();
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            restRequest.AddHeader(this.myKey, Convert.ToString(this.subComponent.First().Result));
            return restRequest;
        }
    }
}