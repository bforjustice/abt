using MessageBuilders.Components.GeneralComponents;
using MessageBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBuilders.Components.RestComponents
{
    public class UtcTimeMSec : ValueComponent
    {
        private string timeFormat = string.Empty;

        public UtcTimeMSec(string key, string value) : 
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
                    long nEpochTicks = 0;
                    long nUnixTimeStamp = 0;
                    long nNowTicks = 0;
                    long nowMiliseconds = 0;
                    string sNonce = "";
                    DateTime DateTimeNow = DateTime.UtcNow;

                    nEpochTicks = new DateTime(1970, 1, 1).Ticks;
                    nNowTicks = DateTimeNow.Ticks;
                    nowMiliseconds = DateTimeNow.Millisecond;

                    nUnixTimeStamp = ((nNowTicks - nEpochTicks) / TimeSpan.TicksPerSecond);

                    sNonce = nUnixTimeStamp.ToString() + nowMiliseconds.ToString("D03");

                    this.myValue = (Convert.ToInt64(sNonce)).ToString(this.timeFormat);
                }

                return this.myValue;
            }
        }
    }
}
