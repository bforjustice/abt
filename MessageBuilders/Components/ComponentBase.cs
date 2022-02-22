namespace MessageBuilders.Components
{
    using LogTrace.Interfaces;
    using MessageBuilders.Interfaces;
    using System.Collections.Generic;

    public abstract class ComponentBase<T>
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CommLogger");

        protected IList<IBlockComponent<T>> subComponent = new List<IBlockComponent<T>>();

        protected string myKey = string.Empty;

        protected virtual void Do()
        {
        }

        public abstract T Result { get; }

        public string Key => this.myKey;

        public virtual void SetSubComponent(IBlockComponent<T> comp)
        {
            if (!subComponent.Contains(comp))
            {
                subComponent.Add(comp);
            }
        }
    }
}