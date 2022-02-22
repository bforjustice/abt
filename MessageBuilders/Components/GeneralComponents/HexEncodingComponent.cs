namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System.Text;

    public class HexEncodingComponent : ComponentBase<object>, IBlockComponent<object>
    {
        private readonly char[] HEX_DIGITS = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        private string myEncodedValue = string.Empty;

        public HexEncodingComponent(string key)
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

            StringBuilder sb = new StringBuilder();
            byte[] hashRes = (byte[])hashAlg.Result;
            for (int i = 0; i < hashRes.Length; i++)
            {
                sb.Append(HEX_DIGITS[(hashRes[i] & 0xf0) >> 4] + ""
                        + HEX_DIGITS[hashRes[i] & 0xf]);
            }
            this.myEncodedValue = sb.ToString();
        }
    }
}