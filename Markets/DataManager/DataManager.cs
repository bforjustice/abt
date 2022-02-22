namespace Markets.Models
{
    using Configuration;
    using DataModels;
    using Newtonsoft.Json.Linq;
    using System;

    public class DataManager
    {
        private readonly string TAG = "DataManager";

        public Data OrderData { get; set; } = null;
        public Data PriceDiffData { get; set; } = null;
        public Data MaxBenefitData { get; set; } = null;

        public Data Orderbook { get; set; } = null;

        public Settings Settings { get; set; }

        public DataManager(Settings settings)
        {
            this.Settings = settings;
        }

        public bool setOrderbook(JObject obj)
        {
            try
            {
                for (int depth = 0; depth < Constants.ORDERBOOK_SIZE; depth++)
                {
                    //// 여기서 해당 마켓에 대한 다른 모든 마켓에 대한 OrderBook 설정
                    //// 10개되면 이게 계산은 되나
                    this.Orderbook.setDepth(ORDERBOOK_SIDE.ASK, Convert.ToDouble(obj["asks"][depth][0]), Convert.ToDouble(obj["asks"][depth][1]), depth);
                    this.Orderbook.setDepth(ORDERBOOK_SIDE.BID, Convert.ToDouble(obj["bids"][depth][0]), Convert.ToDouble(obj["bids"][depth][1]), depth);
                }
                return true;
            }
            catch (Exception e)
            {
                //myLogger.Error("setOrderbook exception!\n" + e);
                return false;
            }
        }

        public void initDataManager()
        {
            if (OrderData == null)
            {
                OrderData = new Data();
            }
            if (PriceDiffData == null)
            {
                PriceDiffData = new Data();
            }
            if (MaxBenefitData == null)
            {
                MaxBenefitData = new Data();
            }

            if (this.Orderbook == null) ;
            {
                Orderbook = new Data();
            }
        }
    }
}