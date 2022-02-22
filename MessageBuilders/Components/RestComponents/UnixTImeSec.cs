using MessageBuilders.Components.GeneralComponents;
using MessageBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBuilders.Components.RestComponents
{
    public class UnixTimeSec : ValueComponent
    {
        public UnixTimeSec(string key, string value) : 
            base(key, string.Empty)
        {
        }

        public override object Result
        {
            get
            {
                if (this.myValue.Equals(string.Empty))
                {
                    // Just Once 
                    this.myValue = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                }

                return this.myValue;
            }
        }
    }
}
