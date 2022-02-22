namespace DataUpdater.Interfaces
{
    using DataModels;
    using System.Collections.Concurrent;

    public interface IObserverTrade
    {
        void Publish(ConcurrentStack<OrderInfo> buySide, ConcurrentStack<OrderInfo> sellSide);
    }
}