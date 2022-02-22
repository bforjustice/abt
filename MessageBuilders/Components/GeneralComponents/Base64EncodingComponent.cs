namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;

    public class Base64EncodingComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myEncodedValue = string.Empty;

        public Base64EncodingComponent(string key)
        {
            this.myKey = key;
        }

        public override object Result
        {
            get
            {
                this.Do();
                return this.myEncodedValue;
            }
        }

        protected override void Do()
        {
            IBlockComponent<object> hashAlg = null;
            foreach (IBlockComponent<object> item in this.subComponent)
            {
                if (item.Key.Equals("Hash"))
                {
                    hashAlg = item;
                    break;
                }
            }

            this.myEncodedValue = System.Convert.ToBase64String((byte[])hashAlg.Result);
        }
    }
}