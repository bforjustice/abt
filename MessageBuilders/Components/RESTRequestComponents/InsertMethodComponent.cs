namespace MessageBuilders.Components.RESTRequestComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using RestSharp;
    using RestSharpMethod = RestSharp.Method;
    using System;
    using System.Linq;
    using System.Net;

    public class InsertMethodComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        public override object Result => throw new NotImplementedException();

        public InsertMethodComponent(string key)
        {
            this.myKey = key;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            httpWebRequest.Method = Convert.ToString(this.subComponent.First().Result);
            return httpWebRequest;
        }

        public HttpWebRequest Create()
        {
            throw new NotImplementedException();
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            restRequest.Method = this.Method(Convert.ToString(this.subComponent.First().Result));
            return restRequest;
        }

        private RestSharpMethod Method(string method)
        {
            RestSharpMethod other;
            switch (method)
            {
                case "GET":
                    other = RestSharpMethod.GET;
                    break;
                case "POST":
                    other = RestSharpMethod.POST;
                    break;
                case "PUT":
                    other = RestSharpMethod.PUT;
                    break;
                case "DELETE":
                    other = RestSharpMethod.DELETE;
                    break;
                case "HEAD":
                    other = RestSharpMethod.HEAD;
                    break;
                case "OPTIONS":
                    other = RestSharpMethod.OPTIONS;
                    break;
                case "PATCH":
                    other = RestSharpMethod.PATCH;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("method", method, null);
            }

            return other;
        }
    }
}