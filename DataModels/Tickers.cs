namespace DataModels
{
    using Configuration;
    using System.Collections.Generic;
    using System.Linq;

    public class Tickers : ItemBase
    {
        public REQUEST_TYPE Type { get; set; }

        public string Identifier { get; set; }

        public void SetSymbols(string symbol)
        {
            if (string.Empty.Equals(symbol))
            {
                return;
            }

            this.mySymbols.Add(symbol);
        }

        public string GetCoin(string symbol)
        {
            return this.myCoins.Where(s => s.Equals(symbol)).FirstOrDefault<string>();
        }

        public void SetCoin(string rawCoin)
        {
            if (string.Empty.Equals(rawCoin))
            {
                return;
            }

            this.myCoins.Add(rawCoin);
        }

        private IList<string> mySymbols = new List<string>();

        private IList<string> myCoins = new List<string>();
    }
}