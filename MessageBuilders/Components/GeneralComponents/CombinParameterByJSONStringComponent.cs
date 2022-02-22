namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;

    public class CombinParameterByJSONStringComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myQueryString = string.Empty;

        private JObject myJsonObj;

        public override object Result
        {
            get
            {
                this.Do();
                return myQueryString;
            }
        }

        public CombinParameterByJSONStringComponent(string key)
        {
            this.myKey = key;
        }

        protected override void Do()
        {
            this.myQueryString = string.Empty;
            this.myJsonObj = null;
            JObject obj = new JObject();

            if (this.subComponent.Count.Equals(0))
            {
                this.myQueryString = string.Empty;
                return;
            }

            foreach (IBlockComponent<object> item in this.subComponent)
            {
                if (item.Result.GetType().Equals(typeof(bool)))
                {
                    obj.Add(item.Key, new JValue(Convert.ToBoolean(item.Result)));
                }
                else if (item.Result.GetType().Equals(typeof(long)))
                {
                    obj.Add(item.Key, new JValue(Convert.ToInt64(item.Result)));
                }
                else if (item.Result.GetType().Equals(typeof(int)))
                {
                    obj.Add(item.Key, new JValue(Convert.ToInt32(item.Result)));
                }
                else if (item.Result.GetType().Equals(typeof(double)))
                {
                    obj.Add(item.Key, new JValue(Convert.ToDouble(item.Result)));
                }
                else
                {
                    obj.Add(item.Key, new JValue(item.Result as string));
                }
            }

            this.myQueryString = JsonConvert.SerializeObject(obj);
        }
    }
}