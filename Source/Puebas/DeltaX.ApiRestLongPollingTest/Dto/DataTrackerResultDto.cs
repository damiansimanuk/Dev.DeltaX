namespace DeltaX.ApiRestLongPollingTest.Dto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DataTrackerResultDto<T>
    {
        public DataTrackerResultDto(IEnumerable<DataTracker<T>> items)
        {
            if (items != null && items.Any())
            {
                First = items.Min(i => i.Updated);
                Last = items.Max(i => i.Updated);
                Items = items.ToArray();
            }
        }

        public IEnumerable<DataTracker<T>> Items { get; set; }
        
        public DateTimeOffset? First { get; set; }
        public DateTimeOffset? Last{ get; set; } 
    }
}
