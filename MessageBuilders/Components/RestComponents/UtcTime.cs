using MessageBuilders.Components.GeneralComponents;
using MessageBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBuilders.Components.RestComponents
{
    public class UtcTime : ValueComponent
    {
        private string timeFormat = string.Empty;

        public UtcTime(string key, string value) : 
            base(key, string.Empty)
        {
            this.timeFormat = value;
        }

        public override object Result
        {
            get
            {
                if (this.myValue.Equals(string.Empty))
                {
                    // Just Once 
                    this.myValue = DateTime.UtcNow.ToString(this.timeFormat);
                }

                return this.myValue;
            }
        }
    }
}
