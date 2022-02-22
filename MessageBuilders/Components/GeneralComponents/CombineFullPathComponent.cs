namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;

    public class CombineFullPathComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myFullPath = string.Empty;

        public CombineFullPathComponent(string key)
        {
            this.myKey = key;
        }

        public override object Result
        {
            get
            {
                this.Do();
                return this.myFullPath;
            }
        }

        protected override void Do()
        {
            this.myFullPath = string.Empty;
            foreach (IBlockComponent<object> item in this.subComponent)
            {
                this.myFullPath = string.Concat(this.myFullPath, Convert.ToString(item.Result));
            }
        }
    }
}