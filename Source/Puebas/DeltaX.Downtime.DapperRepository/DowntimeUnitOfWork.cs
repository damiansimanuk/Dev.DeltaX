namespace DeltaX.Downtime.DapperRepository
{
    using DeltaX.Domain.Common.Entities;
    using DeltaX.Domain.Common.Events;
    using DeltaX.Domain.Common.Repositories;
    using MediatR;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class DowntimeUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly List<IAggregateRoot> changeTracker;
        private readonly IMediator mediator;

        public DowntimeUnitOfWork(IDbConnection dbConnection)
        {
            changeTracker = new List<IAggregateRoot>();
            DbConnection = dbConnection;
        }

        public IDbConnection DbConnection { get; private set; }


        public IDbTransaction DbTransaction { get; private set; }


        public IEnumerable<IAggregateRoot> ChangeTracker => changeTracker;

        public void SaveChanges()
        {
            CommitTransaction();

            var domainEvents = GetDomainEvents();
            ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                mediator?.Publish(domainEvent);
            }
        }

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

            ClearDomainEvents();

            if (DbConnection != null)
            {
                DbConnection.Dispose();
                DbConnection = null;
            }
        }

        public void AddChangeTracker(IAggregateRoot entity)
        {
            changeTracker.Add(entity);
        }

        public IEnumerable<INotificationEto> GetDomainEvents()
        {
            return changeTracker?.SelectMany(e => e.GetDomainEvents());
        }

        public void ClearDomainEvents()
        {
            changeTracker?.ForEach(e => e.ClearDomainEvents());
        }
    }
}
