namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;

    public class BitEncodingComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myEncodedValue = string.Empty;

        public BitEncodingComponent(string key)
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
                if (item.Key.Equals("Hash") || item.Key.Equals("HashPassphrase") || item.Key.Equals("PayloadHash"))
                {
                    hashAlg = item;
                    break;
                }
            }

            this.myEncodedValue = System.BitConverter.ToString((byte[])hashAlg.Result).Replace("-", string.Empty).ToLower();
        }
    }
}