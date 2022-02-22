namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;

    public class DeleteMethodComponent : ComponentBase<object>, IBlockComponent<object>
    {
        public DeleteMethodComponent(string key)
        {
            this.myKey = key;
        }

        public override object Result => "DELETE";
    }
}