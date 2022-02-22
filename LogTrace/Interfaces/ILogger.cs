namespace LogTrace.Interfaces
{
    public interface ILogger
    {
        void Info(string msg);

        void Debug(string msg);

        void Warn(string msg);

        void Error(string msg);

        void Fatal(string msg);
    }
}