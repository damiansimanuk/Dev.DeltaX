using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaX.ReactiveDataTest
{
    public class CacheObservable<T, K>
        where K : notnull
    {
        ObservableCollection<List<DataTracker<T>>> cache;
        Func<T, K> keySelector;

        public CacheObservable(Func<T, K> keySelector)
        {
            this.keySelector = keySelector;
            this.cache = new ObservableCollection<List<DataTracker<T>>>();
        }

        protected void AddCache(IEnumerable<T> items, NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Add)
        {
            cache.Add(items.Select(item => new DataTracker<T>(item, action)).ToList());
        }

        protected int RemoveAll(Predicate<DataTracker<T>> match)
        {
            var count = 0;
            foreach (var itms in cache.ToList())
            {
                count += itms.RemoveAll(match);
                if (!itms.Any())
                {
                    cache.Remove(itms);
                }
            }
            return count;
        }

        protected int RemoveAll(T item)
        {
            return RemoveAll(i => keySelector(i.item)?.Equals(keySelector(item)) == true);
        }

        public void Add(T item)
        {
            RemoveAll(item);
            AddCache(new[] { item }, NotifyCollectionChangedAction.Add);
        }

        public void Remove(T item, bool force = false)
        {
            var count = RemoveAll(item);
            if (count > 0 || force)
            {
                AddCache(new[] { item }, NotifyCollectionChangedAction.Remove);
            }
        }

        public void Replace(T item, bool force = false)
        {
            var count = RemoveAll(item);
            if (count > 0 || force)
            {
                AddCache(new[] { item }, NotifyCollectionChangedAction.Replace);
            }
        }

        public Task<List<DataTracker<T>>> WaitResultsAsunc(
            Func<DataTracker<T>, bool> filter,
            CancellationToken? token = null)
        {
            token ??= CancellationToken.None;

            return Task.Run(() =>
            {
                var resetEvent = new ManualResetEventSlim();
                var results = cache.SelectMany(e => e.Where(filter)).ToList();
                results ??= new List<DataTracker<T>>();

                if (results.Any())
                {
                    return results;
                }

                void Cache_CollectionChanged(object s, NotifyCollectionChangedEventArgs e)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (List<DataTracker<T>> items in e.NewItems)
                        {
                            if (items.Any(filter))
                            {
                                results.AddRange(items.Where(filter));
                            }
                        }

                        if (results.Any())
                        {
                            resetEvent.Set();
                        }
                    }
                }

                cache.CollectionChanged += Cache_CollectionChanged;
                resetEvent.Wait(TimeSpan.FromSeconds(20), token.Value);
                cache.CollectionChanged -= Cache_CollectionChanged;

                return results;
            }, token.Value);
        }
    }
}
