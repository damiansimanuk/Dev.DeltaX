using Dapper;
using DeltaX.Downtime.Domain;
using DeltaX.Downtime.Domain.ProcessAggregate;
using DeltaX.Downtime.Repository.Mappers;
using DeltaX.Domain.Common.Entities;
using DeltaX.Domain.Common.Repositories;
using DeltaX.Repository.Common.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaX.Downtime.Repository
{
    public class ProcessHistoryRepository : IProcessHistoryRepository
    {
        private DowntimeRepositoryMapper mapper; 
        private ProcessHistoryUnitOfWork processHistoryUnitOfWork;
        private TableQueryFactory tableQuery;

        public ProcessHistoryRepository(ProcessHistoryUnitOfWork processHistoryUnitOfWork,
            DowntimeRepositoryMapper mapper,
            TableQueryFactory tableQuery)
        {
            this.processHistoryUnitOfWork = processHistoryUnitOfWork;
            this.mapper = mapper;
            this.tableQuery = tableQuery;
        }

        private IDbConnection DbConnection => processHistoryUnitOfWork.DbConnection;

        private IDbTransaction DbTransaction => processHistoryUnitOfWork.DbTransaction;

        public IUnitOfWork UnitOfWork => processHistoryUnitOfWork;


        public Task DeleteAsync(Guid id)
        {
            // var sql = $"DELETE " +
            //     $"FROM {ProcessHistoryDto.TableName} " +
            //     $"WHERE {nameof(ProcessHistoryDto.Id)} = @ProcessHistoryId";

            var sql = tableQuery.GetDeleteQuery<ProcessHistoryDto>();

            DbConnection.Execute(sql, new { Id = id }, DbTransaction);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(ProcessHistory entity )
        { 
            

            // DbConnection.Execute(sql, new { ProcessHistoryId = entity.Id }, DbTransaction);

            return Task.CompletedTask;
        }

        public async Task<ProcessHistory> GetAsync(Guid id, bool includeDetails = false)
        {
            ProcessHistoryDto item = default;

            // var sql = $"SELECT " +
            //     $"{string.Join(", ", item.GetColumns().Select(c => c.Key))} " +
            //     $"FROM {item.GetTableName()} " +
            //     $"WHERE {nameof(ProcessHistoryDto.Id)} = @ProcessHistoryId";

            var query = tableQuery.GetSingleQuery<ProcessHistoryDto>();
            item = await DbConnection.QueryFirstAsync<ProcessHistoryDto>(query, new ProcessHistoryDto { Id = id }, DbTransaction);

            item.Interruption = await GetInterruptionHistoryDtoAsync(item.Id);

            if (!string.IsNullOrEmpty(item.ProductSpecificationCode))
            {
                item.ProductSpecification = await GetProductSpecificationDtoAsync(item.ProductSpecificationCode);
            }

            return mapper.Map<ProcessHistory>(item);
        }

        public async Task<long> GetCountAsync()
        {
           // var sql = $"SELECT count(*) " +
           //   $"FROM {ProcessHistoryDto.TableName} ";

            var tableName = tableQuery.GetTable<ProcessHistoryDto>().Name;
            var sql = $"SELECT count(*) FROM {tableName}";

            sql = tableQuery.GetCountQuery<ProcessHistoryDto>();
             
            var count = await DbConnection.ExecuteScalarAsync<long>(sql, DbTransaction);
            return count;
        }

        public async Task<ProductSpecificationDto> GetProductSpecificationDtoAsync(string code)
        { 
            var sql = tableQuery.GetSingleQuery<ProductSpecificationDto>("WHERE Code = @Code");
            var item = await DbConnection.QueryFirstAsync<ProductSpecificationDto>(sql, new { Code = code }, DbTransaction);
            return item;
        }

        public async Task<ProductSpecification> GetProductSpecificationAsync(string code)
        {
            var item = await GetAsync<ProductSpecificationDto>($"{nameof(ProductSpecificationDto.Code)} = @Code", new { Code = code });
            return mapper.Map<ProductSpecification>(item);
        }

        private async Task<InterruptionHistoryDto> GetInterruptionHistoryDtoAsync(Guid processHistoryId)
        {
            var sql = $"SELECT " +
                $"{string.Join(", ", InterruptionHistoryDto.GetColumns().Select(c => c.Key))} " +
                $"FROM {InterruptionHistoryDto.TableName} " +
                $"WHERE {nameof(InterruptionHistoryDto.ProcessHistoryId)} = @ProcessHistoryId";

            var item = await DbConnection.QueryFirstAsync<InterruptionHistoryDto>(sql, new { ProcessHistoryId = processHistoryId }, DbTransaction);

            return item;
        } 

        public async Task<ProcessHistory> InsertAsync(ProcessHistory entity)
        {
            var item = mapper.Map<ProcessHistoryDto>(entity);

            // Chek or Insert ProductSpecification
            if (entity.ProductSpecification != null)
            {
                item.ProductSpecificationCode = item.ProductSpecification.Code;

                var productSpecification = GetProductSpecificationAsync(item.ProductSpecification.Code);
                if (productSpecification == null)
                { 
                    await InsertAsync(item.ProductSpecification);
                }
            }

            // Insert ProcessHistory 
            await InsertAsync(item);

            // Insert InterruptionHistory
            if (entity.Interruption != null)
            {
                item.Interruption.ProcessHistoryId = item.Id; 
                item.Interruption.Id = await InsertAsync(item.Interruption);
            }

            return mapper.Map<ProcessHistory>(item);
        }

        private Task<int> InsertAsync<T>(T item) where T : IEntityDto
        {
            var tableName = item.GetTableName();
            var columns = item.GetColumns(false);

            var sql = $"INSERT " +
                $"INTO {tableName} " +
                $"({string.Join(", ", columns.Select(e => e.Key))}) " +
                $"VALUES ({string.Join(", ", columns.Select(e => "@" + e.Key))}) " +
                $"; SELECT SCOPE_IDENTITY()";

            return DbConnection.ExecuteScalarAsync<int>(sql, columns, DbTransaction);
        }

        public Task<T> GetAsync<T>(string whereCondition, object whereParam) where T : IEntityDto
        {
            T item = default;
            var tableName = item.GetTableName();
            var columns = item.GetColumns(false);

            var sql = $"SELECT " +
                $"{string.Join(", ", columns.Select(c => c.Key))} " +
                $"FROM {tableName} " +
                $"WHERE {whereCondition}";

            return DbConnection.QueryFirstAsync<T>(sql, whereParam, DbTransaction);
        }


        private async Task<int> InsertAsync(string tableName, IDictionary<string, object> columns)
        { 
            var sql = $"INSERT " +
                $"INTO {tableName} " +
                $"({string.Join(", ", columns.Select(e => e.Key))}) " +
                $"VALUES ({string.Join(", ", columns.Select(e => "@" + e.Key))}) " +
                $"; SELECT SCOPE_IDENTITY()";
              
            var id = await DbConnection.ExecuteScalarAsync<int>(sql, columns , DbTransaction); 
            return id;
        }

        public Task<ProcessHistory> UpdateAsync(ProcessHistory entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessHistory>> GetListAsync(bool includeDetails = false )
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessHistory>> GetPagedListAsync(int skipCount, int maxResultCount, string filter, string sorting, bool includeDetails = false )
        {
            throw new NotImplementedException();
        }
    }
}
