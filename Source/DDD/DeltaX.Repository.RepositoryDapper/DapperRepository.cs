namespace DeltaX.Repository.DapperRepository
{
    using DeltaX.Domain.Common.Repositories;
    using DeltaX.Repository.Common.Table;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapper;

    public class DapperRepository
    {
        TableQueryFactory queryFactory;
        IUnitOfWork uk;

        public DapperRepository(IUnitOfWork unitOfWork, TableQueryFactory queryFactory)
        {
            this.queryFactory = queryFactory;
            this.uk = unitOfWork;
        }

        public Task DeleteAsync<TEntity>(TEntity entity)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetDeleteQuery<TEntity>();
            return uk.DbConnection.ExecuteAsync(query, entity, uk.DbTransaction);
        }

        public Task DeleteAsync<TEntity>(string whereClause, object param)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetDeleteQuery<TEntity>(whereClause);
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
                var fieldId = await uk.DbConnection.ExecuteScalarAsync(query, item, uk.DbTransaction);

                // Set Property Value  
                var propertyColumn = identityColumn.GetPropertyInfo();
                propertyColumn.SetValue(item, Convert.ChangeType(fieldId, propertyColumn.PropertyType));
            }
            else
            {
                await uk.DbConnection.ExecuteAsync(query, item, uk.DbTransaction);
            }

            return item;
        }

        public Task<Tkey> InsertAsync<TEntity, Tkey>(TEntity item, IEnumerable<string> fieldsToInsert = null)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetInsertQuery<TEntity>(fieldsToInsert);
            query += "; " + queryFactory.DialectQuery.IdentityQueryFormatSql;

            return uk.DbConnection.ExecuteScalarAsync<Tkey>(query, item, uk.DbTransaction);
        }

        public Task<TEntity> GetAsync<TEntity>(object param)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetSingleQuery<TEntity>();
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
            return uk.DbConnection.QueryFirstOrDefaultAsync<TEntity>(query, param, uk.DbTransaction);
        }

        public Task<IEnumerable<TEntity>> GetListAsync<TEntity>(int skipCount = 0, int rowsPerPage = 1000,
            string whereClause = null, string orderByClause = null, object param = null)
           where TEntity : ITableDto
        {
            var query = queryFactory.GetPagedListQuery<TEntity>(skipCount, rowsPerPage, whereClause, orderByClause);
            return uk.DbConnection.QueryAsync<TEntity>(query, param, uk.DbTransaction);
        }

        public Task<int> UpdateAsync<TEntity>(string whereClause, object param, IEnumerable<string> fieldsToSet = null)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetUpdateQuery<TEntity>(whereClause, fieldsToSet);

            return uk.DbConnection.ExecuteAsync(query, param, uk.DbTransaction);
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<string> fieldsToSet = null)
           where TEntity : ITableDto
        {
            var query = queryFactory.GetUpdateQuery<TEntity>(null, fieldsToSet);

            return uk.DbConnection.ExecuteAsync(query, entity, uk.DbTransaction);
        }

        public Task<long> GetCountAsync<TEntity>(TEntity entity)
           where TEntity : ITableDto
        {
            var query = queryFactory.GetCountQuery<TEntity>();
            return uk.DbConnection.ExecuteScalarAsync<long>(query, entity, uk.DbTransaction);
        }

        public Task<long> GetCountAsync<TEntity>(string whereClause, object param)
            where TEntity : ITableDto
        {
            var query = queryFactory.GetCountQuery<TEntity>(whereClause);
            return uk.DbConnection.ExecuteScalarAsync<long>(query, param, uk.DbTransaction);
        }
    }
}
