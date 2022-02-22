namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class MD5Component : ComponentBase<object>, IBlockComponent<object>
    {
        private string sKey = string.Empty;

        private byte[] myHashValue = null;

        public MD5Component(string key)
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
                        sData = Convert.ToString(item.Result) + sKey;
                        break;
                }
            }

            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                this.myHashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(sData));
            }
        }
    }
}