namespace Communication
{
    using Communication.Interfaces;
    using DataModels;
    using System.Collections.Generic;

    public abstract class CommunicatorBase
    {
        private IList<IJobSubscriber> communicationPeers = new List<IJobSubscriber>();

        public void Notify(APIResult result)
        {
            foreach (IJobSubscriber subscriber in this.communicationPeers)
            {
                subscriber.PublishJob(result);
            }
        }

        public void Subscribe(IJobSubscriber subscriber)
        {
            if (this.communicationPeers.Contains(subscriber))
            {
                return;
            }

            this.communicationPeers.Add(subscriber);
        }

        public void Unsubscribe(IJobSubscriber subscriber)
        {
            if (this.communicationPeers.Contains(subscriber))
            {
                this.communicationPeers.Remove(subscriber);
            }
        }
    }
}