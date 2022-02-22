namespace DataModels
{
    using Common;
    using Configuration;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;

    public class Settings
    {
        private IDictionary<COIN_TYPE, CoinParamGroup> myCoinSettings;

        public static Settings CreateMarketSettings(IDictionary<string, string> coreOptionDatas)
        {
            Settings settings = new Settings();

            settings.Enabled = Convert.ToBoolean(coreOptionDatas[$"Market_Enable"]);
            settings.PendingTime = Convert.ToInt64(coreOptionDatas["PendingTimeInSecond"]) * 1000;
            settings.OrderBookLimit = Convert.ToInt32(coreOptionDatas["OrderbookLimit"]);
            settings.OrderBookPeriod = Convert.ToInt32(coreOptionDatas["OrderbookPeriod"]);

            settings.TakerFee = Convert.ToDouble(coreOptionDatas[$"Taker_Fee"]);
            settings.MakerFee = Convert.ToDouble(coreOptionDatas[$"Maker_Fee"]);

            foreach (COIN_TYPE coinType in Enum.GetValues(typeof(COIN_TYPE)))
            {
                settings.SetOrderUnit(coinType, Convert.ToDouble(coreOptionDatas[$"Order_Unit_{coinType.ToString()}"]));
                settings.SetMinTradeValue(coinType, Convert.ToDouble(coreOptionDatas[$"Min_Trade_Value_{coinType.ToString()}"]));
                settings.SetPriceDiffThreshold(coinType, Convert.ToDouble(coreOptionDatas[$"PriceDiffThreshold_{coinType.ToString()}"]));
                settings.SetMarginThreshold(coinType, Convert.ToDouble(coreOptionDatas[$"Min_Devouring_{coinType.ToString()}"]));
                settings.SetBound(coinType, Convert.ToDouble(coreOptionDatas[$"Quotation_{coinType.ToString()}"]));

                if (settings.GetMinTradeValue(coinType) == 0)
                {
                    settings.SetIsSupport(coinType, false);
                }
                else
                {
                    settings.SetDecimalLength(coinType, CommonApi.getDecimalLength(settings.GetMinTradeValue(coinType)));
                    settings.SetIsSupport(coinType, true);
                }
            }

            settings.Leverage = Convert.ToInt64(coreOptionDatas["Leverage"]);
            settings.API_KEY = coreOptionDatas["API_KEY"].ToString();
            settings.SECRET_KEY = coreOptionDatas["SECRET_KEY"].ToString();

            return settings;
        }

        public Settings()
        {
            initialize();
        }

        private void initialize()
        {
            ////ToDo
            //TradeFrequency[(int)COIN_MARKET.BINANCE] = 20 * 1000;
            this.myCoinSettings = new Dictionary<COIN_TYPE, CoinParamGroup>();

            foreach (var coinMarket in Enum.GetValues(typeof(COIN_MARKET)))
            {
                MarketEnabled = true;
            }
        }

        public COIN_TYPE CoinMode;

        public bool Auto { get; set; } = true;

        public bool Enabled { get; set; } = true;

        public double BenefitRatio { get; set; } = 100;

        public int Bound { get; set; } = 0;

        public long PendingTime = 20 * 1000;

        public long TradeFrequency;

        public double MakerFee { get; set; }

        public double TakerFee { get; set; }

        public bool AutoFee;

        public int OrderBookLimit { get; set; } = 0;

        public int OrderBookPeriod { get; set; } = 200;

        public int RestartPeriod { get; set; } = 3600 * 1000;

        public bool MarketEnabled { get; set; }

        public long Leverage { get; set; }

        public string API_KEY { get; set; }

        public string SECRET_KEY { get; set; }

        public POSITION_TYPE POSITION_TYPE { get; set; }

        public int LiveMarketCount
        {
            get
            {
                //// Todo
                int count = 0;
                return count;
            }
        }

        public void SetOrderUnit(COIN_TYPE coinType, double value)
        {
            if (!this.myCoinSettings.ContainsKey(coinType))
            {
                this.myCoinSettings.Add(coinType, new CoinParamGroup(coinType));
            }

            this.myCoinSettings[coinType].OrderUnit = value;
        }

        public double GetOrderUnit(COIN_TYPE coinType)
        {
            if (this.myCoinSettings.ContainsKey(coinType))
            {
                return this.myCoinSettings[coinType].OrderUnit;
            }

            throw new KeyNotFoundException();
        }

        public void SetMinTradeValue(COIN_TYPE coinType, double value)
        {
            if (!this.myCoinSettings.ContainsKey(coinType))
            {
                this.myCoinSettings.Add(coinType, new CoinParamGroup(coinType));
            }

            this.myCoinSettings[coinType].MinTradeValue = value;
        }

        public double GetMinTradeValue(COIN_TYPE coinType)
        {
            if (this.myCoinSettings.ContainsKey(coinType))
            {
                return this.myCoinSettings[coinType].MinTradeValue;
            }

            throw new KeyNotFoundException();
        }

        public void SetDecimalLength(COIN_TYPE coinType, int value)
        {
            if (!this.myCoinSettings.ContainsKey(coinType))
            {
                this.myCoinSettings.Add(coinType, new CoinParamGroup(coinType));
            }

            this.myCoinSettings[coinType].DecimalLength = value;
        }

        public int GetDecimalLength(COIN_TYPE coinType)
        {
            if (this.myCoinSettings.ContainsKey(coinType))
            {
                return this.myCoinSettings[coinType].DecimalLength;
            }

            throw new KeyNotFoundException();
        }

        public void SetIsSupport(COIN_TYPE coinType, bool value)
        {
            if (!this.myCoinSettings.ContainsKey(coinType))
            {
                this.myCoinSettings.Add(coinType, new CoinParamGroup(coinType));
            }

            this.myCoinSettings[coinType].IsSupport = value;
        }

        public bool GetIsSupport(COIN_TYPE coinType)
        {
            if (this.myCoinSettings.ContainsKey(coinType))
            {
                return this.myCoinSettings[coinType].IsSupport;
            }

            throw new KeyNotFoundException();
        }

        public void SetPriceDiffThreshold(COIN_TYPE coinType, double value)
        {
            if (!this.myCoinSettings.ContainsKey(coinType))
            {
                this.myCoinSettings.Add(coinType, new CoinParamGroup(coinType));
            }

            this.myCoinSettings[coinType].PriceDiffThreshold = value;
        }

        public double GetPriceDiffThreshold(COIN_TYPE coinType)
        {
            if (this.myCoinSettings.ContainsKey(coinType))
            {
                return this.myCoinSettings[coinType].PriceDiffThreshold;
            }

            throw new KeyNotFoundException();
        }

        public void SetMarginThreshold(COIN_TYPE coinType, double value)
        {
            if (!this.myCoinSettings.ContainsKey(coinType))
            {
                this.myCoinSettings.Add(coinType, new CoinParamGroup(coinType));
            }

            this.myCoinSettings[coinType].MarginThreshold = value;
        }

        public double GetMarginThreshold(COIN_TYPE coinType)
        {
            if (this.myCoinSettings.ContainsKey(coinType))
            {
                return this.myCoinSettings[coinType].MarginThreshold;
            }

            throw new KeyNotFoundException();
        }

        public void SetBound(COIN_TYPE coinType, double value)
        {
            if (!this.myCoinSettings.ContainsKey(coinType))
            {
                this.myCoinSettings.Add(coinType, new CoinParamGroup(coinType));
            }

            this.myCoinSettings[coinType].Bound = value;
        }

        public double GetBound(COIN_TYPE coinType)
        {
            if (this.myCoinSettings.ContainsKey(coinType))
            {
                return this.myCoinSettings[coinType].Bound;
            }

            throw new KeyNotFoundException();
        }

        public JObject Dump()
        {
            JObject obj = new JObject();

            obj.Add("Enable", this.Enabled);
            obj.Add("PendingTime", this.PendingTime);
            obj.Add("OrderBookLimit", this.OrderBookLimit);
            obj.Add("OrderBookPeriod", this.OrderBookPeriod);
            obj.Add("TakerFee", this.TakerFee);
            obj.Add("MakerFee", this.MakerFee);
            obj.Add("Leverage", this.Leverage);
            obj.Add("API_KEY", this.API_KEY);
            obj.Add("SECRET_KEY", this.SECRET_KEY);

            foreach (COIN_TYPE type in this.myCoinSettings.Keys)
            {
                JObject coinObj = new JObject();

                coinObj.Add("Bound", this.GetBound(type));
                coinObj.Add("OrderUnit", this.GetOrderUnit(type));
                coinObj.Add("MinTradeValue", this.GetMinTradeValue(type));
                coinObj.Add("DecimalLength", this.GetDecimalLength(type));
                coinObj.Add("IsSupport", this.GetIsSupport(type));
                coinObj.Add("MarginThreshold", this.GetMarginThreshold(type));
                coinObj.Add("PriceDiffThreshold", this.GetPriceDiffThreshold(type));

                obj.Add(type.ToString(), coinObj);
            }

            return obj;
        }
    }
}