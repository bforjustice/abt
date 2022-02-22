namespace MessageBuilders.Components
{
    using LogTrace.Interfaces;
    using MessageBuilders.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RestComponentBase<T> : IBlockComponent<T>
    {
        public string Key => this.myKey;

        public T Result => this.myContent;

        protected virtual void Do() { }

        protected string myKey = string.Empty;

        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CommLogger");

        protected IList<IBlockComponent<T>> myComponents = new List<IBlockComponent<T>>();

        protected T myContent;

        public virtual void SetSubComponent(IBlockComponent<T> comp)
        {
            if (!this.myComponents.Contains(comp))
            {
                this.myComponents.Add(comp);
            }
        }
    }
}
