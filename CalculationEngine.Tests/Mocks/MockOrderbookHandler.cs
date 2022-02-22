using OrderBookHandler.Interfaces;
using System;

namespace CalculationEngine.Tests.Mocks
{
    public class MockOrderbookHandler : IObservableOrderbooks
    {
        public void Subscribe(IObserverOrderbooks subscriber)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IObserverOrderbooks subscriber)
        {
            throw new NotImplementedException();
        }
    }
}