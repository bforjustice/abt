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
    public class AddQueryComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        public override object Result => throw new NotImplementedException();

        public AddQueryComponent(string key)
        {
            this.myKey = key;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            throw new NotImplementedException();
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            foreach (IBlockComponent<object> item in this.subComponent)
            {
                restRequest.AddQueryParameter(item.Key, item.Result.ToString());
            }

            return restRequest;
        }
    }
}
