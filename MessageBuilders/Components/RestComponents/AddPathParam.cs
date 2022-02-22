namespace MessageBuilders.Components.RestComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Components.Interfaces;
    using MessageBuilders.Components.RESTBuilder.OptionModels;
    using MessageBuilders.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class AddPathParam : IParamWorker<RequestOptions>
    {
        private readonly string myKey = string.Empty;

        private RequestOptions myResult;

        private IList<IBlockComponent<object>> myComponents;

        public RequestOptions Result => this.myResult;

        public string Key => this.myKey;

        public AddPathParam(string key)
        {
            this.myKey = key;
            this.myComponents = new List<IBlockComponent<object>>();
        }

        public void Do(RequestOptions opts)
        {
            this.myResult = opts;

            foreach (IBlockComponent<object> item in this.myComponents)
            {
                this.myResult.PathParameters.Add(item.Key, item.Result.ToString());
            }
        }

        public void SetComponent(IBlockComponent<object> param)
        {
            if (!this.myComponents.Contains(param))
            {
                this.myComponents.Add(param);
            }
        }
    }
}