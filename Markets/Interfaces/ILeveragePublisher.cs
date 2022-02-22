namespace Markets.Interfaces
{
    using DataModels;
    using System.Collections.Generic;

    public interface ILeveragePublisher
    {
        void Subscribe(ILeverageSubscriber subscriber);

        void Unsubscribe(ILeverageSubscriber subscriber);

        void Notify(IList<Position> positions);
    }
}