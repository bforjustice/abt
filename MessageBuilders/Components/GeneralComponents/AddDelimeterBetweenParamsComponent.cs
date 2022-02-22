namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;
    using System.Text;

    public class AddDelimeterBetweenParamsComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myPreSignatureValue = string.Empty;

        public override object Result
        {
            get
            {
                this.Do();
                return this.myPreSignatureValue;
            }
        }

        public AddDelimeterBetweenParamsComponent(string key)
        {
            this.myKey = key;
        }

        protected override void Do()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IBlockComponent<object> item in this.subComponent)
            {
                sb.Append(Convert.ToString(item.Result));
                sb.Append("\n");
            }

            this.myPreSignatureValue = sb.ToString().Trim('\n');
        }
    }
}