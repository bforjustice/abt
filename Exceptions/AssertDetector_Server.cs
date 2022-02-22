namespace Exceptions
{
    using Common;
    using Configuration;
    using LogTrace.Interfaces;
    using System;
    using System.Diagnostics;

    public class AssertDetector_Server
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("AssertDetectorLogger");

        private static AssertDetector_Server sAssertDetector = null;

        private long[] mLastPrivateErrorTime = new long[Constants.MARKET_COUNT];

        public static AssertDetector_Server getInstance()
        {
            if (sAssertDetector == null)
            {
                sAssertDetector = new AssertDetector_Server();
            }
            return sAssertDetector;
        }

        private AssertDetector_Server()
        {
        }

        public void addUnhandledExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(unhandledExceptionHandler);
        }

        public void onPrivateError(COIN_MARKET market, string msg)
        {
            long currentTime = TimeManager.UtcTimeMS();
            if (currentTime - mLastPrivateErrorTime[(int)market] > 5 * 1000)
            {
                //// ToDo - Logging Exception
                //DBController.getInstance().updateExceptionLog($"[{TimeManager.SyncronizedTime}] [{market}] {msg}");
                mLastPrivateErrorTime[(int)market] = currentTime;
            }
        }

        private static void unhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            myLogger.Fatal($"unhandledExceptionHandler sender: {sender}");
            myLogger.Fatal($"unhandledExceptionHandler caught: {e}");
            myLogger.Fatal($"unhandledExceptionHandler error stacktrace: {e.StackTrace}");
            myLogger.Fatal($"Runtime terminating: {args.IsTerminating}");

            //// ToDo - Logging Exception
            //DBController.getInstance().updateExceptionLog($"{Settings.CoinMode} client error!!! {e}");

            try
            {
                foreach (Process process in Process.GetProcessesByName("Devourer"))
                {
                    myLogger.Warn("Kill process, name = " + process.ProcessName);
                    process.Kill();
                }
            }
            catch (Exception e1)
            {
                //Log.e("Unhandled", "killProcess exception!" + e1);
            }
        }
    }
}