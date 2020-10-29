namespace DeltaX.Domain.Common.Events
{  
    public interface IEventStore
    {
        void Add(INotificationEto eventItem);

        void Remove(INotificationEto eventItem);

        void Clear();
    }
}
