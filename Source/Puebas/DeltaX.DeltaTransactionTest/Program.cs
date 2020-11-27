using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DeltaX.DeltaTransactionTest
{
    class Program
    { 

        static void Main(string[] args)
        {
            Console.WriteLine("Test DeltaTransaction Init!"); 

            // Debug.Assert(DeltaTransaction.Current == null, "DeltaTransaction.Current is not null");
            // 
            // var t = DeltaTransaction.InitializeDeltaTransaction();
            // t.TransactionCompleted += T_TransactionCompleted;
            // t.TransactionStarted += T_TransactionStarted;
            // Debug.Assert(DeltaTransaction.Current != null, "DeltaTransaction.Current is null");
            // 
            // var r = Task.Run(() => task());
            // 
            // t.Start();
            // Debug.Assert(DeltaTransaction.Current.IsActive(), "DeltaTransaction.Current is not active");
            // 
            // t.Rollback();
            // 
            // r.Wait();
        }
         
    }
}

