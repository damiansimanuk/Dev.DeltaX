using DeltaX.Database;
using DeltaX.Domain.Common.Repositories;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaX.Downtime.Repository
{
    public class ProcessHistoryUnitOfWork : IUnitOfWork, IDisposable
    {

        public ProcessHistoryUnitOfWork(IDbConnection dbConnection)
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
