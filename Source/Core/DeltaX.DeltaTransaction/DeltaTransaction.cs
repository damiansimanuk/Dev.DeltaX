using System;

namespace DeltaX.Transaction
{ 

    public class DeltaTransaction : IDisposable
    { 
        private bool disposed;

        [ThreadStatic]
        public static DeltaTransaction Current;

        public static DeltaTransaction InitializeDeltaTransaction()
        {
            try
            {
                Current?.Dispose();
            }
            finally
            {
                Current = null;
            }

            Current = new DeltaTransaction();
            return Current;
        } 

        public DeltaTransaction()
        {
            disposed = false;
            Status = DeltaTransactionStatus.Unknown;
            Identifier = Guid.NewGuid().ToString("N");
        }

        ~DeltaTransaction()
        {
            Dispose();
        }


        public delegate void DeltaTransactionCompletedEventHandler(object sender, DeltaTransactionEventArgs transactionEventArgs);

        public event DeltaTransactionCompletedEventHandler TransactionCompleted;
        public event DeltaTransactionCompletedEventHandler TransactionStarted;

        public DeltaTransactionStatus Status { get; private set; }

        public string Identifier { get; private set; }

        public bool IsActive()
        {
            return Status == DeltaTransactionStatus.Active;
        }

        public void Start()
        {
            if (Status != DeltaTransactionStatus.Unknown)
            {
                throw new Exception(string.Format("Transaction in status {0}", Status));
            }

            Status = DeltaTransactionStatus.Active;
            TransactionStarted?.Invoke(this, new DeltaTransactionEventArgs(this));
        }

        public void Commit()
        {
            Status = DeltaTransactionStatus.Committed;
            TransactionCompleted?.Invoke(this, new DeltaTransactionEventArgs(this));
            DoDispose();
        }

        public void Rollback()
        {
            Status = DeltaTransactionStatus.Aborted;
            TransactionCompleted?.Invoke(this, new DeltaTransactionEventArgs(this));
            DoDispose();
        }

        public void Dispose()
        {
            DoDispose();
            GC.SuppressFinalize(this);
        }

        private void DoDispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            try
            {
                if (Status == DeltaTransactionStatus.Active)
                {
                    Rollback();
                }
            }
            finally
            {
                if (this == Current)
                {
                    Current = null;
                }
            }            
        }
    }
}
