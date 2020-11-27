using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Transactions;

namespace DeltaX.Cache
{
    public class TransactionItems<TItem>
    {
        private ConcurrentDictionary<string, List<TItem>> transactionItems;
        private ILogger logger;
        [ThreadStatic]
        private static Transaction subscribedTransaction;

        public delegate void TransactionCompletedEventHandler(object sender, IEnumerable<TItem> items);
        public event TransactionCompletedEventHandler TransactionCommitted;
        public event TransactionCompletedEventHandler TransactionAborted;

        public TransactionItems(ILogger logger = null)
        {
            this.logger = logger;
            transactionItems = new ConcurrentDictionary<string, List<TItem>>();
        }

        public string Identifier => Transaction.Current?.TransactionInformation?.LocalIdentifier;

        public bool IsTransactionActive(Transaction transaction = null)
        {
            transaction ??= Transaction.Current;
            return transaction != null;
        }

        public bool Add(Predicate<TItem> match, TItem item)
        {
            if (!IsTransactionActive())
            {
                return false;
            }

            var transId = Identifier;
            List<TItem> items = null;

            if (transId == null)
            {
                return false;
            }

            if (!transactionItems.TryGetValue(transId, out items))
            {
                logger?.LogDebug("TransId:{0} Generate new List", transId);
                transactionItems[transId] = items = new List<TItem>();
            }

            SubscribeTransactionCompleted();

            logger?.LogDebug("TransId:{0} Add Item:{1}", transId, item);
            items.RemoveAll(a => match(a));
            items.Add(item);
            return true;
        }

        private void SubscribeTransactionCompleted()
        {
            if (subscribedTransaction == null || subscribedTransaction != Transaction.Current)
            {
                Transaction.Current.TransactionCompleted -= OnTransactionCompleted;
                Transaction.Current.TransactionCompleted += OnTransactionCompleted;
                subscribedTransaction = Transaction.Current;
            }
        }


        private void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            var transaction = e.Transaction;
            var transId = transaction.TransactionInformation.LocalIdentifier;

            try
            {
                logger?.LogDebug("TransId:{0} OnTransactionCompleted Status:{1}", transId, transaction.TransactionInformation.Status);
                var items = GetAll(transId, true);
                if (transaction.TransactionInformation.Status == TransactionStatus.Committed)
                {
                    TransactionCommitted?.Invoke(this, items);
                }
                else
                {
                    TransactionAborted?.Invoke(this, items);
                }
            }
            finally
            {
                transaction.TransactionCompleted -= OnTransactionCompleted;
                subscribedTransaction = null;
                transactionItems.TryRemove(transId, out _);
            }
        }

        public IEnumerable<TItem> GetAll(string transactionId = null, bool remove = false)
        {
            transactionId ??= Identifier;
            List<TItem> items = null;

            if (transactionId != null && transactionItems.TryGetValue(transactionId, out items))
            {
                if (remove)
                {
                    logger?.LogDebug("TransId:{0} Get and Removed all items", transactionId);
                    transactionItems.TryRemove(transactionId, out _);
                }
            }
            return items;
        }
    }
}

