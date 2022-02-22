namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;

    public class GetMethodComponent : ComponentBase<object>, IBlockComponent<object>
    {
        public GetMethodComponent(string key)
        {
            this.myKey = key;
        }

        public override object Result => "GET";
    }
}