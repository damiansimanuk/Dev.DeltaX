using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaX.Cache
{
    public class CacheItems<TItem> where TItem : class
    {
        private TimeSpan expirationTime;
        private object locker = new object();
        private List<CacheItemDto<TItem>> cacheItems;
        private TransactionItems<CacheItemDto<TItem>> transactionItems;
        private CacheFilteredCollection<CacheItemDto<TItem>> filteredCollection;
        private ILogger logger;

        public CacheItems(
            TransactionItems<CacheItemDto<TItem>> transactionItems,
            IEnumerable<TItem> items = null,
            TimeSpan? expirationTime = null,
            CacheFilteredCollection<CacheItemDto<TItem>> filteredCollection = null,
            ILogger logger = null)
        {
            this.expirationTime = expirationTime ?? TimeSpan.FromHours(1);

            if (transactionItems != null)
            {
                this.transactionItems = transactionItems;
                this.transactionItems.TransactionCommitted += OnTransactionCommitted;
                this.transactionItems.TransactionAborted += OnTransactionAborted;
            }
            
            InitializeCacheItems(items ?? new List<TItem>());

            this.filteredCollection = filteredCollection;
            this.logger = logger;
        }


        public void InitializeCacheItems(IEnumerable<TItem> items)
        {
            items = items ?? throw new ArgumentException("Parametter is null", nameof(items));

            cacheItems = items
                .Select(i => new CacheItemDto<TItem> { Updated = DateTime.Now, Status = CacheItemStatus.Initialized, Value = i })
                .ToList();

            UpdateAllCacheItems();
            logger?.LogDebug("CacheBase InitializeCacheItems items:{0}", cacheItems.Count());
        }

        private IEnumerable<CacheItemDto<TItem>> CacheTransactionItems;

        private void UpdateAllCacheItems()
        {
            var txItem = transactionItems?.GetAll();
            CacheTransactionItems = txItem == null ? cacheItems : txItem.Union(cacheItems).ToList();
        }
        
        private void ResetCacheItems()
        { 
            CacheTransactionItems = cacheItems;
        }

        public IEnumerable<TItem> Items
        {
            get
            {
                return CacheTransactionItems
                    .Select(e => e.Value)
                    .ToArray();
            }
        }


        public TItem GetCacheItem(Func<TItem, bool> match, Func<TItem> operationLoadItem)
        {
            lock (locker)
            {
                var elem = CacheTransactionItems.FirstOrDefault(item => match(item.Value));
                if (elem != null && elem.Updated + expirationTime > DateTime.Now)
                {
                    return elem.Value;
                }

                logger?.LogDebug("CacheBase GetCacheItem operationLoadItem");
                var item = operationLoadItem();
                AddTransactionalItem(match, item, CacheItemStatus.Added);

                return item;
            }
        }

        public IEnumerable<TItem> GetCacheItems(Func<TItem, bool> predicate)
        {
            lock (locker)
            {
                var elem = CacheTransactionItems.Where(e => predicate(e.Value));
                if (elem.Any())
                {
                    return elem.Select(e => e.Value).ToArray();
                }

                return null;
            }
        }

        public void UpdateCacheItem(Func<TItem, bool> match, TItem newItem, CacheItemStatus status = CacheItemStatus.Updated)
        {
            AddTransactionalItem(match, newItem, status);
        }

        public TItem RemoveCacheItem(Func<TItem, bool> match)
        {
            lock (locker)
            {
                var item = cacheItems.FirstOrDefault(item => match(item.Value));
                if (item != null)
                {
                    AddTransactionalItem(match, item.Value, CacheItemStatus.Removed);
                    return item.Value;
                }
                return default(TItem);
            }
        }

        public void ClearCacheExpired()
        {
            lock (locker)
            {
                var expireTimeBegin = DateTime.Now - expirationTime;
                var items = cacheItems.Where(item => item.Updated < expireTimeBegin);
                foreach (var item in items.ToArray())
                {
                    logger?.LogDebug("CacheBase remove expired item:{0}", item);
                    cacheItems.Remove(item);
                }
            }
        }

        public void ClearCache()
        {
            lock (locker)
            {
                cacheItems.Clear();
            }
        }

        private void AddTransactionalItem(Func<TItem, bool> match, TItem item, CacheItemStatus status)
        {
            _ = item ?? throw new ArgumentException("item is null", "item");

            Console.WriteLine("DbCache: Adding data to cache: {0}.", item.ToString());
            var newItem = new CacheItemDto<TItem> { Updated = DateTime.Now, Status = status, Value = item, MatchItem = match };

            lock (locker)
            {
                if (transactionItems?.Add(e => match(e.Value) && e.Status == status, newItem) == true)
                {
                    UpdateAllCacheItems();
                }
                else
                {
                    DoUpdateItems(new[] { newItem });
                }
            }
        }

        private void DoUpdateItems(IEnumerable<CacheItemDto<TItem>> items)
        {
            _ = items ?? throw new ArgumentException("item is null", "item");

            logger?.LogDebug("CacheBase OnTransactionCompleted add {0} items", items.Count());

            lock (locker)
            {
                foreach (var item in items)
                {
                    switch (item.Status)
                    {
                        case CacheItemStatus.Removed:
                            cacheItems.RemoveAll(i => i.MatchItem(item.Value));
                            break;
                        case CacheItemStatus.Created:
                        case CacheItemStatus.Added:
                        case CacheItemStatus.Updated:
                            var e = cacheItems.FirstOrDefault(i => i.MatchItem(item.Value));
                            if (e != null)
                            {
                                e.Value = item.Value;
                            }
                            else
                            {
                                cacheItems.Add(item);
                            }
                            break;
                    }
                }

                filteredCollection?.Publish(items);
            }
        }

        private void OnTransactionAborted(object sender, IEnumerable<CacheItemDto<TItem>> items)
        {
            ResetCacheItems();
        }

        private void OnTransactionCommitted(object sender, IEnumerable<CacheItemDto<TItem>> items)
        {
            ClearCacheExpired();
            if (items != null && items.Any())
            {
                DoUpdateItems(items);
            }
            ResetCacheItems();
        } 
    }

}
