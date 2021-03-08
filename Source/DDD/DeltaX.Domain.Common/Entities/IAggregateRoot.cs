namespace DeltaX.Domain.Common.Entities
{
    using DeltaX.Domain.Common.Events;
    using System.Collections.Generic;

    public interface IAggregateRoot : IEntity
    {  
        void AddDomainEvent(INotificationEto eventItem);

        void RemoveDomainEvent(INotificationEto eventItem);

        void ClearDomainEvents();

        IEnumerable<INotificationEto> GetDomainEvents();
    }
}
