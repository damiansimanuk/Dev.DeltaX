namespace DeltaX.Domain.Common.Repositories
{
    using System.Data;


    public interface IUnitOfWork
    { 
        IDbConnection DbConnection { get; }

        IDbTransaction DbTransaction { get; }

        bool IsInTransaction();

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}
