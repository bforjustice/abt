namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class SHA256Component : ComponentBase<object>, IBlockComponent<object>
    {
        private string sKey = string.Empty;

        private byte[] myHashValue = null;

        public SHA256Component(string key)
        {
            this.myKey = key;
        }

        public override object Result
        {
            get
            {
                this.Do();
                return myHashValue;
            }
        }

        protected override void Do()
        {
            string sKey = string.Empty;
            string sData = string.Empty;
            foreach (IBlockComponent<object> item in this.subComponent)
            {
                switch (item.Key)
                {
                    case "SecretKey":
                        sKey = Convert.ToString(item.Result);
                        break;

                    default:
                        sData = Convert.ToString(item.Result);
                        break;
                }
            }

            using (HMACSHA256 hmacsha256 = new HMACSHA256(new UTF8Encoding().GetBytes(sKey)))
            {
                this.myHashValue = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(sData));
            }
        }
    }
}