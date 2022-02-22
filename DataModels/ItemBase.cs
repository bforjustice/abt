namespace DataModels
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class ItemBase
    {
        public Exception Exception { get; set; }

        public DATA_SOURCE DataSource { get; set; }

        public REQUEST_STATE State { get; set; }

        public COIN_MARKET Market { get; set; }
    }
}
