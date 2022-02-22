namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;

    public class CombineQueryString : ComponentBase<object>, IBlockComponent<object>
    {
        private string myQueryString = string.Empty;

        public override object Result
        {
            get
            {
                this.Do();
                return myQueryString;
            }
        }

        public CombineQueryString(string key)
        {
            this.myKey = key;
        }

        protected override void Do()
        {
            this.myQueryString = string.Empty;

            if (this.subComponent.Count == 0)
            {
                this.myQueryString = string.Empty;
            }

            foreach (IBlockComponent<object> item in this.subComponent)
            {
                if (item is CombineQueryString || item is UriEncodingComponent || item is MappedQueryValueComponent)
                {
                    this.myQueryString = string.Concat(this.myQueryString, Convert.ToString(item.Result), "&");
                }
                else
                {
                    if (item.Result.GetType().Equals(typeof(bool)))
                    {
                        this.myQueryString = string.Concat(this.myQueryString, item.Key, "=", Convert.ToString(item.Result).ToLower(), "&");
                    }
                    else
                    {
                        this.myQueryString = string.Concat(this.myQueryString, item.Key, "=", Convert.ToString(item.Result), "&");
                    }
                }
            }

            this.myQueryString = this.myQueryString.Trim('&');
        }
    }
}