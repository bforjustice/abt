namespace LogTrace.Interfaces
{
    using log4net.Config;
    using System.Collections.Generic;
    using System.IO;

    public class LogTraceService : ILogTraceService
    {
        private static string configuratioFilePath = "..//..//..//ConfigurationFiles//LogTrace//loggerConfig.xml";

        private static object syncLock = new object();

        private static LogTraceService instance = null;

        private IDictionary<string, ITracer> tracers = new Dictionary<string, ITracer>();

        private IDictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();

        public static LogTraceService Instance
        {
            get
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new LogTraceService();
                    }
                }

                return instance;
            }
        }

        private LogTraceService()
        {
            XmlConfigurator.Configure(new FileInfo(configuratioFilePath));
        }

        public ITracer GetTracer(string tracerName)
        {
            ITracer tracer = null;
            if (!this.tracers.TryGetValue(tracerName, out tracer))
            {
                tracer = new Tracer(tracerName);
                tracers.Add(tracerName, tracer);
            }

            return tracer;
        }

        public ILogger GetLogger(string loggerName)
        {
            ILogger logger = null;
            if (!this.loggers.TryGetValue(loggerName, out logger))
            {
                logger = new Logger(loggerName);
                loggers.Add(loggerName, logger);
            }

            return logger;
        }
    }
}