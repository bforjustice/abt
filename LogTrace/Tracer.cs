namespace LogTrace
{
    using log4net;
    using LogTrace.Interfaces;

    public class Tracer : ITracer
    {
        private ILog tracer = null;

        public Tracer(string domain)
        {
            this.tracer = LogManager.GetLogger(domain);
        }

        public void Info(string msg)
        {
            this.tracer.Info(msg);
        }

        public void Debug(string msg)
        {
            this.tracer.Debug(msg);
        }

        public void Warn(string msg)
        {
            this.tracer.Warn(msg);
        }

        public void Error(string msg)
        {
            this.tracer.Error(msg);
        }

        public void Fatal(string msg)
        {
            this.tracer.Fatal(msg);
        }
    }
}