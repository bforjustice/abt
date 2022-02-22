namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;

    public class CombineParameterComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myCombineValue = string.Empty;

        public CombineParameterComponent(string key)
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
                this.myCombineValue = string.Concat(this.myCombineValue, Convert.ToString(item.Result));
            }
        }
    }
}