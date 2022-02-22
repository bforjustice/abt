using MessageBuilders.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MessageBuilders.Components.RESTRequestComponents
{
    public class AddUpperCaseEncodedQueryComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        public override object Result => throw new NotImplementedException();

        public AddUpperCaseEncodedQueryComponent(string key)
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
                string context = item.Result.ToString();
                string encodedContext = this.UrlEncode(context);

                restRequest.AddQueryParameter(item.Key, encodedContext, false); 
            }

            return restRequest;
        }

        private string UrlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString(), Encoding.UTF8).Length > 1)
                {
                    builder.Append(HttpUtility.UrlEncode(c.ToString(), Encoding.UTF8).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }
    }
}
