namespace Common
{
    using System;
    using System.Reflection;

    public abstract class CommonApi
    {
        public static double cutDecimalNumber(double number, int chiper)
        {
            return Math.Round(number, chiper);
        }

        public static int getDecimalLength(double number)
        {
            string[] temp = number.ToString().Split('.');
            if (temp.Length == 2)
            {
                return temp[1].Length;
            }
            return 0;
        }

        public static void restart()
        {
            var fileName = Assembly.GetExecutingAssembly().Location;
            System.Diagnostics.Process.Start(fileName);
            Environment.Exit(0);
        }

        public static bool compareDecimalLength(double qty, double minTradeValue)
        {
            return getDecimalLength(qty) <= getDecimalLength(minTradeValue);
        }
    }
}