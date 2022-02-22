namespace MessageBuilders.Components.GeneralComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class SHA512Component : ComponentBase<object>, IBlockComponent<object>
    {
        private string sKey = string.Empty;

        private byte[] myHashValue = null;

        public SHA512Component(string key)
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
            string sData = string.Empty;

            sData = Convert.ToString(this.subComponent.First().Result);

            using (SHA512 sha512Hash = SHA512.Create())
            {
                this.myHashValue = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(sData));
            }
        }
    }
}