namespace Markets.Interfaces
{
    using DataModels;
    using System.Collections.Generic;

    public interface IPositionPublisher
    {
        void Subscribe(IPositionSubscriber subscriber);

        void Unsubscribe(IPositionSubscriber subscriber);

        void Notify(IList<Position> positions);
    }
}