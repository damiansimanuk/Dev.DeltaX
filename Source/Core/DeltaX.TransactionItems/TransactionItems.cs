﻿using DeltaX.Transaction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DeltaX.Transaction
{
    public class TransactionItems<TItem>
    {
        private ConcurrentDictionary<string, List<TItem>> transactionItems = new ConcurrentDictionary<string, List<TItem>>();
        private ILogger logger; 
        private DeltaTransactionFactory transactionFactory;
        [ThreadStatic]
        private static DeltaTransaction subscribedTransaction;

        public delegate void TransactionCompletedEventHandler(object sender, IEnumerable<TItem> items);
        public event TransactionCompletedEventHandler TransactionCommitted;
        public event TransactionCompletedEventHandler TransactionAborted;
         
        public TransactionItems(DeltaTransactionFactory transactionFactory)
        {
            this.transactionFactory = transactionFactory; 
        }

        public string Identifier => transactionFactory.Current?.Identifier;

        public bool IsActive(DeltaTransaction transaction = null)
        {
            transaction ??= transactionFactory.Current;
            return transaction.IsActive();
        }

        public bool Add(Predicate<TItem> match, TItem item)
        {
            if (!IsActive())
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
                logger.LogDebug("TransId:{0} Generate new List", transId);
                transactionItems[transId] = items = new List<TItem>();
            }
             
            SubscribeTransactionCompleted();

            logger.LogDebug("TransId:{0} Add Item:{1}", transId, item);
            items.RemoveAll(a => match(a));             
            items.Add(item);
            return true;
        }

        private void SubscribeTransactionCompleted()
        {
            if (subscribedTransaction == null || subscribedTransaction != transactionFactory.Current)
            {
                transactionFactory.Current.TransactionCompleted -= OnTransactionCompleted;
                transactionFactory.Current.TransactionCompleted += OnTransactionCompleted;
                subscribedTransaction = transactionFactory.Current;
            }
        }

        private void OnTransactionCompleted(object sender, DeltaTransactionEventArgs e)
        {
            var transaction = e.Transaction;
            var transId = transaction.Identifier;

            try
            {
                logger.LogDebug("TransId:{0} OnTransactionCompleted Status:{1}", transId, transaction.Status);
                var items = GetAll(transId, true);
                if (transaction.Status == DeltaTransactionStatus.Committed)
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
                    logger.LogDebug("TransId:{0} Get and Removed all items", transactionId);
                    transactionItems.TryRemove(transactionId, out _);
                }
            }
            return items;
        }
    }
}
