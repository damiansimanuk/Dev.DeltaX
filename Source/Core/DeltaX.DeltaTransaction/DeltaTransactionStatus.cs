using System;

namespace DeltaX.Transaction
{ 
    public enum DeltaTransactionStatus
    {
        Unknown = 0,
        Active = 1,
        Committed = 2,
        Aborted = 3
    }
}
