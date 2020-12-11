namespace DeltaX.ApiRestLongPollingTest.Dto
{
    using System;

    public class DataTracker<T>
    {
        public DateTimeOffset Updated { get; set; }
        public T Item { get; set; }

        public DataTracker(T item, DateTimeOffset? updated)
        {
            this.Updated = updated ?? new DateTimeOffset(DateTime.Now);
            this.Item = item;
        }
    }
}

