namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;

    public class CombineRequestPathComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myPath = string.Empty;

        public CombineRequestPathComponent(string key)
        {
            this.myKey = key;
        }

        public override object Result
        {
            get
            {
                this.Do();
                return myPath;
            }
        }

        protected override void Do()
        {
            this.myPath = string.Empty;
            foreach (IBlockComponent<object> item in this.subComponent)
            {
                this.myPath = string.Concat(myPath, Convert.ToString(item.Result));
            }
        }
    }
}