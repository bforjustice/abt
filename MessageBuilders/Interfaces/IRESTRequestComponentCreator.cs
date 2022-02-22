using RestSharp;
using System.Net;

namespace MessageBuilders.Interfaces
{
    public interface IRESTRequestComponentCreator
    {
        HttpWebRequest Do(HttpWebRequest httpWebRequest);

        IRestRequest Do(IRestRequest restRequest);
    }
}