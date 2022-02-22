namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;

    public class ValueComponent : ComponentBase<object>, IBlockComponent<object>
    {
        protected string myValue = string.Empty;

        public ValueComponent(string key, string value)
        {
            this.myKey = key;
            this.myValue = value;
        }

        public override object Result => this.myValue;
    }
}