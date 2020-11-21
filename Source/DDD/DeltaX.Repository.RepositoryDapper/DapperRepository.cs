namespace DeltaX.Repository.DapperRepository
{
    using DeltaX.Domain.Common.Repositories;
    using DeltaX.Repository.Common.Table;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Extensions.Logging;

    public class DapperRepository
    {
        TableQueryFactory queryFactory;
        IUnitOfWork uk;
        ILogger logger;

        public DapperRepository(IUnitOfWork unitOfWork, TableQueryFactory queryFactory, ILogger logger = null)
        {
            this.queryFactory = queryFactory;
            this.uk = unitOfWork;
            this.logger = logger;
        }   

        public Task DeleteAsync<TEntity>(TEntity entity)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetDeleteQuery<TEntity>();
            logger.LogDebug("DeleteAsync query:{query} entity:{@entity}", query, entity);
            return uk.DbConnection.ExecuteAsync(query, entity, uk.DbTransaction);
        }

        public Task DeleteAsync<TEntity>(string whereClause, object param)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetDeleteQuery<TEntity>(whereClause);
            logger.LogDebug("DeleteAsync query:{query} whereClause:{whereClause} param:{@param}", query, whereClause, param);
            return uk.DbConnection.ExecuteAsync(query, param, uk.DbTransaction);
        }


        public async Task<TEntity> InsertAsync<TEntity>(TEntity item, IEnumerable<string> fieldsToInsert = null)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetInsertQuery<TEntity>(fieldsToInsert); 

            var identityColumn = queryFactory.GetTable<TEntity>().GetIdentityColumn();
            if (identityColumn != null)
            {
                query += "; " + queryFactory.DialectQuery.IdentityQueryFormatSql;
                logger.LogDebug("InsertAsync query:{query} item:{@item}", query, item);
                var fieldId = await uk.DbConnection.ExecuteScalarAsync(query, item, uk.DbTransaction);

                // Set Property Value  
                var propertyColumn = identityColumn.GetPropertyInfo();
                propertyColumn.SetValue(item, Convert.ChangeType(fieldId, propertyColumn.PropertyType));
            }
            else
            {
                logger.LogDebug("InsertAsync query:{query} item:{@item}", query, item);
                await uk.DbConnection.ExecuteAsync(query, item, uk.DbTransaction);
            }

            return item;
        }

        public Task<Tkey> InsertAsync<TEntity, Tkey>(TEntity item, IEnumerable<string> fieldsToInsert = null)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetInsertQuery<TEntity>(fieldsToInsert);
            query += "; " + queryFactory.DialectQuery.IdentityQueryFormatSql;
            logger.LogDebug("InsertAsync query:{query} item:{@item}", query, item);

            return uk.DbConnection.ExecuteScalarAsync<Tkey>(query, item, uk.DbTransaction);
        }

        public Task<TEntity> GetAsync<TEntity>(object param)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetSingleQuery<TEntity>();
            logger.LogDebug("GetAsync query:{query} param:{@param}", query, param);

            return uk.DbConnection.QueryFirstOrDefaultAsync<TEntity>(query, param, uk.DbTransaction);
        }

        public Task<TEntity> GetAsync<TEntity>(TEntity entity)
            where TEntity : ITableDto
        {
            return GetAsync<TEntity>((object)entity);
        }

        public Task<TEntity> GetAsync<TEntity>(string whereClause, object param)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetSingleQuery<TEntity>(whereClause);
            logger.LogDebug("GetAsync query:{query} whereClause:{whereClause} param:{@param}", query, whereClause, param);

            return uk.DbConnection.QueryFirstOrDefaultAsync<TEntity>(query, param, uk.DbTransaction);
        }

        public Task<IEnumerable<TEntity>> GetPagedListAsync<TEntity>(int skipCount = 0, int rowsPerPage = 1000,
            string whereClause = null, string orderByClause = null, object param = null)
           where TEntity : ITableDto
        {
            if (!string.IsNullOrEmpty(whereClause))
            {
                if (!whereClause.TrimStart().StartsWith("WHERE", true, null))
                {
                    whereClause = "WHERE " + whereClause.Trim();
                }
            }
            if (!string.IsNullOrEmpty(orderByClause))
            {
                if (!orderByClause.TrimStart().StartsWith("ORDER BY", true, null))
                {
                    orderByClause = "ORDER BY " + orderByClause.Trim();
                }
            }

            var query = queryFactory.GetPagedListQuery<TEntity>(skipCount, rowsPerPage, whereClause, orderByClause);
            logger.LogDebug("GetPagedListAsync query:{query} whereClause:{whereClause} param:{@param}", query, whereClause, param);

            return uk.DbConnection.QueryAsync<TEntity>(query, param, uk.DbTransaction);
        }

        public Task<int> UpdateAsync<TEntity>(string whereClause, object param, IEnumerable<string> fieldsToSet = null)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetUpdateQuery<TEntity>(whereClause, fieldsToSet);
            logger.LogDebug("UpdateAsync query:{query} whereClause:{whereClause} param:{@param}", query, whereClause, param);

            return uk.DbConnection.ExecuteAsync(query, param, uk.DbTransaction);
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<string> fieldsToSet = null)
           where TEntity : ITableDto
        {
            var query = queryFactory.GetUpdateQuery<TEntity>(null, fieldsToSet);
            logger.LogDebug("UpdateAsync query:{query} entity:{@entity}", query, entity);

            return uk.DbConnection.ExecuteAsync(query, entity, uk.DbTransaction);
        }

        public Task<long> GetCountAsync<TEntity>(TEntity entity)
           where TEntity : ITableDto
        {
            var query = queryFactory.GetCountQuery<TEntity>();
            logger.LogDebug("GetCountAsync query:{query} entity:{@entity}", query, entity);

            return uk.DbConnection.ExecuteScalarAsync<long>(query, entity, uk.DbTransaction);
        }

        public Task<long> GetCountAsync<TEntity>(string whereClause, object param)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetCountQuery<TEntity>(whereClause);
            logger.LogDebug("GetCountAsync query:{query} whereClause:{whereClause} param:{@param}", query, whereClause, param);

            return uk.DbConnection.ExecuteScalarAsync<long>(query, param, uk.DbTransaction);
        }
    }

    public class DapperRepository<TEntity> :
        DapperRepository,
        IBasicRepository<TEntity> where TEntity : class, ITableDto
    {
        TableQueryFactory queryFactory;
        IUnitOfWork uk;

        public DapperRepository(IUnitOfWork unitOfWork, TableQueryFactory queryFactory, ILogger logger = null)
            :base(unitOfWork, queryFactory, logger)
        {
            this.queryFactory = queryFactory;
            this.uk = unitOfWork;
        }

        public IUnitOfWork UnitOfWork => uk;

        public Task DeleteAsync(TEntity entity)
        {
            return base.DeleteAsync<TEntity>(entity);
        }

        public Task<TEntity> GetAsync(TEntity entity, bool includeDetails = false)
        {
            return base.GetAsync<TEntity>(entity);
        }

        public Task<long> GetCountAsync()
        {
            return base.GetCountAsync<TEntity>(null);
        }

        public Task<IEnumerable<TEntity>> GetListAsync(bool includeDetails = false)
        {
            return base.GetPagedListAsync<TEntity>();
        }

        public Task<IEnumerable<TEntity>> GetPagedListAsync(int skipCount, int rowsPerPage, string filter, string sorting, 
            object param = null, bool includeDetails = false)
        { 
            return base.GetPagedListAsync<TEntity>(skipCount, rowsPerPage, filter, sorting, param);
        }

        public Task<TEntity> InsertAsync(TEntity entity)
        {
            return base.InsertAsync<TEntity>(entity);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await base.UpdateAsync<TEntity>(entity);
            return entity;
        }
    }
}
