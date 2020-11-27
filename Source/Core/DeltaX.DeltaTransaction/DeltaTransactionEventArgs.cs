using System;

namespace DeltaX.Transaction
{
    public class DeltaTransactionEventArgs : EventArgs
    {
        public DeltaTransactionEventArgs(DeltaTransaction transaction)
        {
            Transaction = transaction;
            Status = transaction.Status;
        }

        public DeltaTransaction Transaction { get; }
        public DeltaTransactionStatus Status { get; }
    }
}
