namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;

    public class PostMethodComponent : ComponentBase<object>, IBlockComponent<object>
    {
        public PostMethodComponent(string key)
        {
            this.myKey = key;
        }

        public override object Result => "POST";
    }
}