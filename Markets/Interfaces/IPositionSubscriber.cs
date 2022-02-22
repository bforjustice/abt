namespace Markets.Interfaces
{
    using DataModels;
    using System.Collections.Generic;

    public interface IPositionSubscriber
    {
        void PublishPosition(IList<Position> positions);
    }
}