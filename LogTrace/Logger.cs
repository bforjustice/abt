namespace LogTrace
{
    using log4net;
    using LogTrace.Interfaces;

    public class Logger : ILogger
    {
        private ILog logger = null;

        public Logger(string domain)
        {
            this.logger = LogManager.GetLogger(domain);
        }

        public void Info(string msg)
        {
            this.logger.Info(msg);
        }

        public void Debug(string msg)
        {
            this.logger.Debug(msg);
        }

        public void Warn(string msg)
        {
            this.logger.Warn(msg);
        }

        public void Error(string msg)
        {
            this.logger.Error(msg);
        }

        public void Fatal(string msg)
        {
            this.logger.Fatal(msg);
        }
    }
}