using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DeltaX.Cache
{
    public class CacheFilteredCollection<TItem>
    {
        private List<FilteredCollection<TItem>> collections = new List<FilteredCollection<TItem>>();
        private Func<TItem, bool> defaultFilter = (item) => { return true; };

        public FilteredCollection<TItem> GetOrAddCollection(string queueName, int queueCapacity, Func<TItem, bool> elementFilter = null)
        {
            FilteredCollection<TItem> collection;

            collection = collections.FirstOrDefault(s => s.QueueName == queueName);
            if (collection == null)
            { 
                collection = new FilteredCollection<TItem>(this, queueName, elementFilter ?? defaultFilter, queueCapacity);

                lock (collections)
                {
                    collections.Add(collection);
                }
            }

            collection.LastRead = DateTime.Now;
            return collection;
        }

        public FilteredCollection<TItem> GetCollection(string queueName)
        {
            return collections.FirstOrDefault(s => s.QueueName == queueName);
        }

        public void Publish(TItem item)
        {
            foreach (var collection in collections.ToArray())
            {
                try
                {
                    collection.Push(item);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("No se puede agregar elementos!");
                }
            }
        }

        public void Publish(IEnumerable<TItem> items)
        {
            foreach (var collection in collections.ToArray())
            {
                try
                {
                    collection.Push(items);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("No se puede agregar elementos!");
                }
            }
        }
         
        public void RemoveCollection(FilteredCollection<TItem> collection)
        {
            lock (collections)
            {
                collections.Remove(collection);
            }
        }
    }

}
