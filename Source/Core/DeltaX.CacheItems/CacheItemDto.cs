using System;

namespace DeltaX.Cache
{
    public class CacheItemDto<TItem> where TItem : class
    { 
        public DateTime Updated { get; internal set; }
        public TItem Value { get; internal set; } 
        public CacheItemStatus Status { get; internal set; }
        public Func<TItem, bool> MatchItem { get; internal set; }
    }

}
