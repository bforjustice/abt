namespace Common
{
    using LogTrace.Interfaces;
    using System;
    using System.Net;
    using System.Net.Sockets;

    public class TimeManager
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("TimeManagerLogger");

        public static TimeManager getInstance()
        {
            if (sTimeManager == null)
            {
                sTimeManager = new TimeManager();
            }
            return sTimeManager;
        }

        private const string TAG = "TimeManager";

        private static long mLastSyncTime = 0;

        private static TimeManager sTimeManager = null;

        private static TimeSpan mTimeDiff = new TimeSpan();

        public static DateTime SyncronizedTime
        {
            get { return DateTime.UtcNow - mTimeDiff; }
        }

        public long ConvertTimeToMs(DateTime DateTimeNow)
        {
            long nEpochTicks = 0;
            long nUnixTimeStamp = 0;
            long nNowTicks = 0;
            long nowMiliseconds = 0;
            string sNonce = "";

            nEpochTicks = new DateTime(1970, 1, 1).Ticks;
            nNowTicks = DateTimeNow.Ticks;
            nowMiliseconds = DateTimeNow.Millisecond;

            nUnixTimeStamp = ((nNowTicks - nEpochTicks) / TimeSpan.TicksPerSecond);

            sNonce = nUnixTimeStamp.ToString() + nowMiliseconds.ToString("D03");

            return (Convert.ToInt64(sNonce));
        }

        public static long UtcTimeMS()
        {
            long nEpochTicks = 0;
            long nUnixTimeStamp = 0;
            long nNowTicks = 0;
            long nowMiliseconds = 0;
            string sNonce = "";
            DateTime DateTimeNow = SyncronizedTime;

            nEpochTicks = new DateTime(1970, 1, 1).Ticks;
            nNowTicks = DateTimeNow.Ticks;
            nowMiliseconds = DateTimeNow.Millisecond;

            nUnixTimeStamp = ((nNowTicks - nEpochTicks) / TimeSpan.TicksPerSecond);

            sNonce = nUnixTimeStamp.ToString() + nowMiliseconds.ToString("D03");

            return (Convert.ToInt64(sNonce));
        }

        public static long UtcTimeSec()
        {
            long nEpochTicks = 0;
            long nUnixTimeStamp = 0;
            long nNowTicks = 0;
            long nowSeconds = 0;
            string sNonce = "";
            DateTime DateTimeNow = SyncronizedTime;

            nEpochTicks = new DateTime(1970, 1, 1).Ticks;
            nNowTicks = DateTimeNow.Ticks;
            nowSeconds = DateTimeNow.Second;

            nUnixTimeStamp = ((nNowTicks - nEpochTicks) / TimeSpan.TicksPerSecond);

            sNonce = nUnixTimeStamp.ToString();

            return (Convert.ToInt64(sNonce));
        }

        public static void syncNetworkTime()
        {
            long currentTime = UtcTimeMS();
            if (currentTime - mLastSyncTime < 60 * 1000)
            {
                return;
            }
            const string ntpServer = "pool.ntp.org";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                var addresses = Dns.GetHostEntry(ntpServer).AddressList;
                var ipEndPoint = new IPEndPoint(addresses[0], 123);

                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();

                ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
                ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

                TimeSpan ts = DateTime.UtcNow - networkDateTime;

                if (ts.Days > 0 || ts.Minutes > 0 || ts.Hours > 0)
                {
                    myLogger.Error($"Sync time with NTP server, time Diff = {ts}");
                    myLogger.Error("TimeDiff value is too big! return");
                    return;
                }
                mTimeDiff = ts;
                myLogger.Error($"Sync time with NTP server, time Diff = {mTimeDiff}");

                mLastSyncTime = currentTime;
            }
            catch (Exception e)
            {
                myLogger.Error("syncNetworkTime exception!!" + e);
            }
            finally
            {
                socket.Close();
            }
        }

        public static long getMsTimeFromDateTime(DateTime DateTimeNow)
        {
            long nEpochTicks = 0;
            long nUnixTimeStamp = 0;
            long nNowTicks = 0;
            long nowMiliseconds = 0;
            string sNonce = "";

            nEpochTicks = new DateTime(1970, 1, 1).Ticks;
            nNowTicks = DateTimeNow.Ticks;
            nowMiliseconds = DateTimeNow.Millisecond;

            nUnixTimeStamp = ((nNowTicks - nEpochTicks) / TimeSpan.TicksPerSecond);

            sNonce = nUnixTimeStamp.ToString() + nowMiliseconds.ToString("D03");

            return (Convert.ToInt64(sNonce));
        }
    }
}