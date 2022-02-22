namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class HmacSHA512Component : ComponentBase<object>, IBlockComponent<object>
    {
        private string sKey = string.Empty;

        private byte[] myHashValue = null;

        public HmacSHA512Component(string key)
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

            using (HMACSHA512 hmacsha512 = new HMACSHA512(new UTF8Encoding().GetBytes(sKey)))
            {
                this.myHashValue = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(sData));
            }
        }
    }
}