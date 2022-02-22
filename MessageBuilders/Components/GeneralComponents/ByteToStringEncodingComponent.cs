namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;

    public class ByteToStringEncodingComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private string myEncodedValue = string.Empty;

        public ByteToStringEncodingComponent(string key)
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

            string sHexStr = "";
            for (int nCnt = 0; nCnt < ((byte[])hashAlg.Result).Length; nCnt++)
            {
                sHexStr += ((byte[])hashAlg.Result)[nCnt].ToString("x2"); // Hex format
            }

            this.myEncodedValue = sHexStr;
        }
    }
}