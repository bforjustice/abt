namespace MessageBuilders.Components.RESTBuilder.OptionModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public class RequestOptions
    {
        public Dictionary<String, String> PathParameters { get; set; }

        public Multimap<String, String> QueryParameters { get; set; }

        public Multimap<String, String> HeaderParameters { get; set; }

        public Dictionary<String, String> FormParameters { get; set; }

        public Dictionary<String, Stream> FileParameters { get; set; }

        public List<Cookie> Cookies { get; set; }

        public Object Data { get; set; }

        public bool RequireApiV4Auth { get; set; }

        public RequestOptions()
        {
            PathParameters = new Dictionary<string, string>();
            QueryParameters = new Multimap<string, string>();
            HeaderParameters = new Multimap<string, string>();
            FormParameters = new Dictionary<string, string>();
            FileParameters = new Dictionary<String, Stream>();
            Cookies = new List<Cookie>();
            RequireApiV4Auth = false;
        }
    }
}
