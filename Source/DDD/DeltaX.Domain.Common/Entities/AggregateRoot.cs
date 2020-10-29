namespace DeltaX.Domain.Common.Entities
{
    using DeltaX.Domain.Common.Events;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    { 
        private readonly ICollection<object> domainEventStore = new Collection<object>();
          
        public void AddDomainEvent(INotificationEto eventItem)
        {
            domainEventStore?.Add(eventItem);
        }

        public void RemoveDomainEvent(INotificationEto eventItem)
        {
            domainEventStore?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            domainEventStore?.Clear();
        }
    }
}