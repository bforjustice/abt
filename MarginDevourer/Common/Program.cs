namespace MarginDevourer
{
    using Configuration;
    using Exceptions;
    using System;
    using System.Reflection;

    internal class Program
    {
        private static void Main(string[] args)
        {
            initAllProcess();
        }

        private static void initAllProcess()
        {
            AssertDetector_Devourer.getInstance().addUnhandledExceptionHandler();
            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            Console.WriteLine("\n\n\n\tMargin Devourer - 최초 실가동 버전");

            COIN_TYPE opCoinMode = COIN_TYPE.BTC;
            bool startMenuEnabled = true;

            while (startMenuEnabled)
            {
                Console.WriteLine($"\t동작할 코인 모드를 입력 ({getCoinMenu()}중 숫자로 입력 후 엔터)");
                try
                {
                    int mode = Convert.ToInt32(Console.ReadLine());
                    if (mode >= 0 && mode < Constants.COIN_COUNT)
                    {
                        opCoinMode = (COIN_TYPE)mode;
                        startMenuEnabled = false;
                    }
                    else
                    {
                        Console.WriteLine("\t정확히 입력하세요");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\t정확히 입력하세요\n{e}");
                }
            }

            MainLogic myMachine = new MainLogic();

            myMachine.initialize(opCoinMode);

            //MainLogic.getInstance().checkLogic();
            myMachine.Do(opCoinMode);
        }

        private static string getCoinMenu()
        {
            string result = string.Empty;
            int i = 0;
            foreach (var coinName in Enum.GetValues(typeof(COIN_TYPE)))
            {
                if (Enum.GetValues(typeof(COIN_TYPE)).Length == ++i)
                {
                    result += $" {(int)coinName}.{coinName} ";
                }
                else
                {
                    result += $" {(int)coinName}.{coinName} /";
                }
            }
            return result;
        }
    }
}