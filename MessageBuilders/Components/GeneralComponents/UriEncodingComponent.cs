namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class UriEncodingComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myEncodedValue = string.Empty;

        public UriEncodingComponent(string key)
        {
            this.myKey = key;
        }

        public override object Result
        {
            get
            {
                this.Do();
                return this.myEncodedValue;
            }
        }

        protected override void Do()
        {
            StringBuilder sb = new StringBuilder();
            SortedDictionary<string, string> sortDic = new SortedDictionary<string, string>();

            IList<string> rawStrArr = Convert.ToString(this.subComponent.First().Result).Split('&');

            foreach (var item in rawStrArr)
            {
                IList<string> para = item.Split('=');

                sortDic.Add(para.First(), UrlEncode(para.Last()));
            }

            foreach (var item in sortDic)
            {
                sb.Append(item.Key).Append("=").Append(item.Value).Append("&");
            }

            this.myEncodedValue = sb.ToString().TrimEnd('&');
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