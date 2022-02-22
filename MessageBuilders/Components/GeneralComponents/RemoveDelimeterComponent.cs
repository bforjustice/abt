namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System.Linq;

    public class RemoveDelimeterComponent : ComponentBase<object>, IBlockComponent<object>
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

        public RemoveDelimeterComponent(string key)
        {
            this.myKey = key;
        }

        protected override void Do()
        {
            this.myPreSignatureValue = (this.subComponent.First().Result as string).Replace("=", "");
        }
    }
}