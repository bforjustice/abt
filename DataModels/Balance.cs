namespace DataModels
{
    using Configuration;
    using DataModels.Exceptions;
    using System;
    using System.Collections.Generic;

    public class Balance : ItemBase
    {
        public static Balance Empty()
        {
            Balance balance = new Balance();
            balance.STATE = REQUEST_STATE.EMPTY;

            return balance;
        }

        public static Balance Create()
        {
            Balance balance = new Balance();
            balance.STATE = REQUEST_STATE.NORMAL;

            return balance;
        }

        public static Balance Create(ApiCallException e)
        {
            Balance balance = new Balance();
            balance.STATE = e.STATE;

            return balance;
        }

        public static Balance Create(Exception e)
        {
            Balance balance = new Balance();
            balance.STATE = REQUEST_STATE.UNKNOWN;

            return balance;
        }

        public REQUEST_STATE STATE { get; private set; }

        public double Balance_USDT { get; set; } = 0;
        public double BalanceLocked_USDT { get; set; } = 0;
        public IDictionary<COIN_TYPE, double> CoinBalance_USDT = new Dictionary<COIN_TYPE, double>();
        public IDictionary<COIN_TYPE, double> CoinBalanceLocked_USDT = new Dictionary<COIN_TYPE, double>();

        public double TotalBalance_USDT()
        {
            double total = 0;
            total += Balance_USDT - BalanceLocked_USDT;

            return total;
        }
    }
}