namespace DeltaX.Downtime.DapperRepository
{
    using DeltaX.Domain.Common.Repositories;
    using System;
    using System.Data;


    public class DowntimeUnitOfWork : IUnitOfWork, IDisposable
    {

        public DowntimeUnitOfWork(IDbConnection dbConnection)
        {
            DbConnection = dbConnection; 
        }

        public IDbConnection DbConnection { get; private set; }


        public IDbTransaction DbTransaction { get; private set; }


        public void BeginTransaction()
        {
            DbTransaction = DbConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (DbTransaction != null)
            {
                DbTransaction.Commit();
                DbTransaction.Dispose();
                DbTransaction = null;
            }
        }

        public bool IsInTransaction()
        {
            return DbTransaction != null;
        }

        public void RollbackTransaction()
        {
            if (DbTransaction != null)
            {
                DbTransaction.Rollback();
                DbTransaction.Dispose();
                DbTransaction = null;
            }
        }

        public void Dispose()
        {
            if (DbTransaction != null)
            {
                DbTransaction.Rollback();
                DbTransaction.Dispose();
                DbTransaction = null;
            }

            if (DbConnection != null)
            {
                DbConnection.Dispose();
                DbConnection = null;
            }
        }
    }
}
