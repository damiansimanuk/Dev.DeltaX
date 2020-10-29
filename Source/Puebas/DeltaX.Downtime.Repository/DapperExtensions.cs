using Dapper;
using DeltaX.Downtime.Repository.Mappers;
using DeltaX.Domain.Common.Entities;
using DeltaX.Domain.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaX.Downtime.Repository
{ 

    public static class DapperExtensions
    {
        // public static string GetTableName<TEntity>(this TEntity entityDto) where TEntity : IEntityDto

        public static string NormalizeWhere(string whereCondition)
        {
            whereCondition = whereCondition.Trim();
            if (whereCondition.ToUpper().StartsWith("WHERE"))
            {
                return whereCondition;
            }
            return "WHERE " + whereCondition;
        }

        public static string NormalizeLimit(int skipCount = 0, int maxResultCount = -1)
        {
            string result = "";

            if (maxResultCount > 0)
            {
                result += $"LIMIT {maxResultCount} ";
            }
            if (skipCount > 0)
            {
                result += $"OFFSET {skipCount} ";
            }

            return result;
        }

        public static Task DeleteAsync<TEntity>(this IUnitOfWork uk, string whereCondition, object whereParam)
            where TEntity : IEntityDto
        {
            TEntity item = default;
            var tableName = item.GetTableName();
            whereCondition = NormalizeWhere(whereCondition); 

            var sql = $"DELETE " +
                $"FROM {tableName} " +
                $"{whereCondition} ";

            uk.DbConnection.Execute(sql, whereParam, uk.DbTransaction);

            return Task.CompletedTask;
        }


        public static Task<int> InsertAsyncc<TEntity>(this IUnitOfWork uk, TEntity item) 
            where TEntity : IEntityDto
        {
            var tableName = item.GetTableName();
            var columns = item.GetColumns(false);

            var sql = $"INSERT " +
                $"INTO {tableName} " +
                $"({string.Join(", ", columns.Select(e => e.Key))}) " +
                $"VALUES ({string.Join(", ", columns.Select(e => "@" + e.Key))}) " +
                $"; SELECT SCOPE_IDENTITY()";

            return uk.DbConnection.ExecuteScalarAsync<int>(sql, columns, uk.DbTransaction);
        }
          
        public static Task<TEntity> GetAsync<TEntity>(this IUnitOfWork uk, string whereCondition, object whereParam) 
            where TEntity : IEntityDto
        {
            TEntity item = default;
            var tableName = item.GetTableName();
            var columns = item.GetColumns(false);
            whereCondition = NormalizeWhere(whereCondition);

            var sql = $"SELECT " +
                $"{string.Join(", ", columns.Select(c => c.Key))} " +
                $"FROM {tableName} " +
                $"{whereCondition} ";
             
            return uk.DbConnection.QueryFirstAsync<TEntity>(sql, whereParam, uk.DbTransaction);
        }

        public static Task<IEnumerable<TEntity>> GetListAsync<TEntity>(this IUnitOfWork uk,
            int skipCount = 0,
            int maxResultCount = -1,
            string whereCondition = null,
            object whereParam = null,
            string appendSql = null)
            where TEntity : IEntityDto
        {
            TEntity item = default;
            var tableName = item.GetTableName();
            var columns = item.GetColumns(false);
            whereCondition = NormalizeWhere(whereCondition);
            var limitCondition = NormalizeLimit(skipCount, maxResultCount);

            var sql = $"SELECT " +
                $"{string.Join(", ", columns.Select(c => c.Key))} " +
                $"FROM {tableName} " +
                $"{whereCondition} {appendSql ?? ""} {limitCondition}";

            return uk.DbConnection.QueryAsync<TEntity>(sql, whereParam, uk.DbTransaction);
        }
    }
}
