using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeltaX.Cache
{

    public class FilteredCollection<TItem> : IDisposable
    { 
        private Func<TItem, bool> itemMatch;
        private BlockingCollection<TItem> collection;
        private CacheFilteredCollection<TItem> cacheFilteredCollection;

        public FilteredCollection(
            CacheFilteredCollection<TItem> cacheFilteredCollection,
            string queueName,
            Func<TItem, bool> itemMatch,
            int queueCapacity = 1000)
        {
            this.cacheFilteredCollection = cacheFilteredCollection;
            this.QueueName = queueName;
            this.collection = new BlockingCollection<TItem>(queueCapacity);
            this.itemMatch = itemMatch;
        }

        public DateTime LastRead { get; set; }

        public string QueueName { get; private set; }

        public bool Push(TItem item)
        {
            return Push(new[] { item });
        }

        public bool Push(IEnumerable<TItem> items)
        {
            items = items.Where(itemMatch);
            if (!items.Any())
            {
                return false;
            }

            lock (collection)
            {
                var count = 0;
                foreach (var item in items)
                {
                    if (collection.Count == collection.BoundedCapacity)
                    {
                        collection.TryTake(out _);
                    }
                    count += collection.TryAdd(item) ? 1 : 0;
                }
                return count > 0;
            }
        }

        public TItem Take(TimeSpan timeout, TItem defaultValue = default(TItem))
        {
            LastRead = DateTime.Now;
            TItem item;

            lock (collection)
            {
                return collection.TryTake(out item, timeout) ? item : defaultValue;
            }
        }

        public IEnumerable<TItem> TakeAll(TimeSpan? timeout = null)
        {
            LastRead = DateTime.Now;
            List<TItem> items = new List<TItem>();
            TItem item;
            timeout ??= TimeSpan.FromSeconds(10);

            lock (collection)
            {
                if (this.collection.TryTake(out item, timeout.Value))
                {
                    items.Add(item);

                    while (this.collection.Count > 0 && this.collection.TryTake(out item, timeout.Value))
                    {
                        items.Add(item);
                    }
                }
                return items;
            }
        }

        public void Dispose()
        {
            cacheFilteredCollection?.RemoveCollection(this);
            cacheFilteredCollection = null;
            collection?.Dispose();
            collection = null;
        }
    }

}
