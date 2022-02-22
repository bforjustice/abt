namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System.Linq;

    public class MappedQueryValueComponent : ComponentBase<object>, IBlockComponent<object>
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

        public MappedQueryValueComponent(string key)
        {
            this.myKey = key;
        }

        protected override void Do()
        {
            this.myQueryString = string.Concat(this.myKey, "=", this.subComponent.First().Result);
        }
    }
}