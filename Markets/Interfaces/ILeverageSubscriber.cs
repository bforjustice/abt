namespace Markets.Interfaces
{
    using DataModels;
    using System.Collections.Generic;

    public interface ILeverageSubscriber
    {
        void PublishLeverage(IList<Position> positions);
    }
}