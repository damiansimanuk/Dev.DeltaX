namespace DeltaX.Domain.Common.Repositories
{
    using DeltaX.Domain.Common.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IReadRepository<TEntity> : IRepository where TEntity : class, IEntity
    {
        Task<List<TEntity>> GetListAsync(bool includeDetails = false);

        Task<long> GetCountAsync();

        Task<List<TEntity>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string filter,
            string sorting,
            bool includeDetails = false);
    }

    public interface IReadRepository<TEntity, TKey> : IReadRepository<TEntity>
        where TEntity : class, IEntity<TKey>
    {
        Task<TEntity> GetAsync(TKey id, bool includeDetails = false);
    }
}