namespace DeltaX.Domain.Common.Entities
{
    using DeltaX.Domain.Common.Events;

    public interface IAggregateRoot
    {  
        void AddDomainEvent(INotificationEto eventItem);

        void RemoveDomainEvent(INotificationEto eventItem);

        void ClearDomainEvents();
    }
}
