using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutofacPruebas
{

    public class SimpleLongPollingLinq<TItem>
    {
        private Func<TItem, bool> filter;

        private BlockingCollection<TItem> items;

        public SimpleLongPollingLinq(int boundedCapacity = 100)
        {
            items = new BlockingCollection<TItem>(boundedCapacity);
        }

        public bool Push(TItem message)
        {
            lock (this.items)
            {
                if (items.Count == items.BoundedCapacity)
                {
                    items.TryTake(out _);
                }
                return items.TryAdd(message);
            }
        }

        public TItem Take(TimeSpan timeout, TItem defaultValue = default(TItem))
        {
            TItem item;
            lock (this.items)
            {
                return items.TryTake(out item, timeout) ? item : defaultValue;
            }
        }

        public IEnumerable<TItem> TakeAll(TimeSpan? timeout = null)
        {
            List<TItem> items = new List<TItem>();
            TItem item;
            timeout ??= TimeSpan.FromSeconds(1);

            lock (this.items)
            {
                while (this.items.Count > 0 && this.items.TryTake(out item, timeout.Value))
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}