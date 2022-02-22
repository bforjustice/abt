using Configuration;

namespace Workers.Interfaces
{
    public interface IWorker
    {
        void Start();

        void Requset(REQUEST_TYPE request, string rawJsonParam);

        void Stop();
    }
}