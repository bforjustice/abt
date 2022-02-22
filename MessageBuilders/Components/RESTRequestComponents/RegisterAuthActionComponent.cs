using MessageBuilders.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MessageBuilders.Components.RESTRequestComponents
{
    public class RegisterAuthActionComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        public override object Result => throw new NotImplementedException();

        public RegisterAuthActionComponent(string key)
        {
            this.myKey = key;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            throw new NotImplementedException();
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            restRequest.OnBeforeRequest = http =>
            {
                try
                {
                    foreach (IBlockComponent<object> item in this.subComponent)
                    {
                        http.Headers.Add(item.Result as HttpHeader);
                    }
                }
                catch
                {
                    throw;
                }
            };

            //foreach (IBlockComponent<object> item in this.subComponent)
            //{
            //    restRequest.AddHeader(item.Key, Convert.ToString(item.Result));
            //}

            return restRequest;
        }
    }
}
