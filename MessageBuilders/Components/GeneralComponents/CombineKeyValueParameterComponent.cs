namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;

    public class CombineKeyValueParameterComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myCombineValue = string.Empty;

        public CombineKeyValueParameterComponent(string key)
        {
            this.myKey = key;
        }

        public override object Result
        {
            get
            {
                this.Do();
                return this.myCombineValue;
            }
        }

        protected override void Do()
        {
            this.myCombineValue = string.Empty;

            foreach (IBlockComponent<object> item in this.subComponent)
            {
                this.myCombineValue = string.Concat(this.myCombineValue, item.Key, item.Result);
            }
        }
    }
}