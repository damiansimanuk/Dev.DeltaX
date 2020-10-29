namespace DeltaX.Domain.Common.Events
{  
    using System.Collections.Generic;


    public class EventStore : IEventStore
    {
        private List<INotificationEto> collection;

        public EventStore()
        {
            lock (collection)
            {
                collection = new List<INotificationEto>();
            }
        }

        public void Add(INotificationEto eventItem)
        {
            lock (collection)
            {
                collection.Add(eventItem);
            }
        }

        public void Clear()
        {
            lock (collection)
            {
                collection.Clear();
            }
        }

        public void Remove(INotificationEto eventItem)
        {
            lock (collection)
            {
                collection.Remove(eventItem);
            }
        }
    }
}
