namespace DeltaX.Domain.Common.Entities
{
    using DeltaX.Domain.Common.Events;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        private ICollection<INotificationEto> domainEventStore;


        public void AddDomainEvent(INotificationEto eventItem)
        {
            domainEventStore ??= new Collection<INotificationEto>();
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

        public IEnumerable<INotificationEto> GetDomainEvents()
        {
            var events = domainEventStore?.ToArray();
            domainEventStore?.Clear();
            return events ?? new INotificationEto[0];
        }
    }
}