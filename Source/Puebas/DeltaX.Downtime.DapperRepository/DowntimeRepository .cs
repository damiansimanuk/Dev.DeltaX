using DeltaX.Domain.Common.Repositories;
using DeltaX.Downtime.Domain;
using DeltaX.Downtime.Domain.ProcessAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DeltaX.Repository.DapperRepository;
using Microsoft.Extensions.Logging;
using DeltaX.Downtime.DapperRepository.Models;
using DeltaX.Utilities;

namespace DeltaX.Downtime.DapperRepository
{
    public class DowntimeRepository : IDowntimeRepository
    {
        private DowntimeTableQueryFactory queryFactory;
        private DowntimeRepositoryMapper mapper;
        private ILogger logger;
        private Repository.DapperRepository.DapperRepository repository;


        public DowntimeRepository(
            IUnitOfWork unitOfWork,
            DowntimeTableQueryFactory queryFactory,
            DowntimeRepositoryMapper mapper,
            ILogger<DowntimeRepository> logger)
        {
            this.UnitOfWork = unitOfWork;
            this.queryFactory = queryFactory;
            this.mapper = mapper;
            this.logger = logger;
            this.repository = new DapperRepository<ProcessHistoryModel>(unitOfWork, queryFactory, logger);
        }


        public IUnitOfWork UnitOfWork { get; }

         
        public Task<ProcessHistory> GetAsync(ProcessHistory entity, bool includeDetails = false)
        { 
            return GetAsync(entity.Id, includeDetails);
        }

        public async Task<ProcessHistory> GetAsync(Guid id, bool includeDetails = false)
        {
            var item = await repository.GetAsync(new ProcessHistoryModel { Id = id });

            item.Interruption = await GetInterruptionDtoByProcessAsync(id);

            if (!string.IsNullOrEmpty(item.ProductSpecificationCode))
            {
                item.ProductSpecification = await GetProductSpecificationDtoAsync(item.ProductSpecificationCode);
            }

            return mapper.Map<ProcessHistory>(item);
        }

        public Task<long> GetCountAsync()
        {
            return repository.GetCountAsync<ProcessHistoryModel>(null);
        }

        private Task<ProductSpecificationModel> GetProductSpecificationDtoAsync(string code)
        {
            return repository.GetAsync<ProductSpecificationModel>(
                @"WHERE Code = @Code",
                new { Code = code });
        }

        private async Task<ProductSpecificationModel> GetOrAddProductSpecificationDtoAsync(ProductSpecificationModel productSpecification)
        {
            // Crea si no existe
            var current = await GetProductSpecificationDtoAsync(productSpecification.Code);
            if (current == null)
            {
                return await repository.InsertAsync(productSpecification);
            }
            return current;
        }

        public async Task<ProductSpecification> GetProductSpecificationAsync(string code)
        {
            var item = await GetProductSpecificationDtoAsync(code);
            return mapper.Map<ProductSpecification>(item);
        } 
        
        private Task<InterruptionHistoryModel> GetInterruptionDtoByProcessAsync(Guid processHistoryId)
        {
            return repository.GetAsync<InterruptionHistoryModel>(
                @"WHERE ProcessHistoryId = @ProcessHistoryId AND Enable = 1",
                new { ProcessHistoryId = processHistoryId.ToString("N") });
        }

        public async Task<InterruptionHistory> GetInterruptionByProcessAsync(Guid processHistoryId)
        {
            var item = await GetInterruptionDtoByProcessAsync(processHistoryId);
            return mapper.Map<InterruptionHistory>(item);
        }

        public async Task<IEnumerable<ProcessHistory>> GetListAsync(bool includeDetails = false)
        {
            var items = await repository.GetPagedListAsync<ProcessHistoryModel>();
            return mapper.Map<List<ProcessHistory>>(items);
        }

        public async Task<IEnumerable<ProcessHistory>> GetPagedListAsync(int skipCount, int maxResultCount,
            string filter = null, string sorting = null, object param = null,  bool includeDetails = false)
        {
            var items = await repository.GetPagedListAsync<ProcessHistoryModel>(skipCount, maxResultCount);
            return mapper.Map<List<ProcessHistory>>(items);
        }

        public Task DeleteAsync(ProcessHistory entity)
        {
            UnitOfWork.AddChangeTracker(entity);
            return repository.DeleteAsync(new ProcessHistoryModel { Id = entity.Id });
        }

        public async Task<ProcessHistory> InsertAsync(ProcessHistory entity)
        {
            logger.LogInformation("InsertAsync ProcessHistory {@entity}", entity);
            
            UnitOfWork.AddChangeTracker(entity);
            
            var item = mapper.Map<ProcessHistoryModel>(entity);

            // Chek or Insert ProductSpecification
            if (item.ProductSpecification != null)
            {
                item.ProductSpecificationCode = item.ProductSpecification.Code;
                item.ProductSpecification = await GetOrAddProductSpecificationDtoAsync(item.ProductSpecification);
            }

            // Insert ProcessHistoryDto
            item = await repository.InsertAsync(item);

            // Insert InterruptionHistory
            if (item.Interruption != null)
            {
                item.Interruption.ProcessHistoryId = item.Id;
                item.Interruption = await repository.InsertAsync(item.Interruption);
            }

            return mapper.Map<ProcessHistory>(item);
        }

        public async Task<ProcessHistory> UpdateAsync(ProcessHistory entity)
        {
            logger.LogInformation("UpdateAsync ProcessHistory {@entity}", entity);

            UnitOfWork.AddChangeTracker(entity);

            var item = mapper.Map<ProcessHistoryModel>(entity);
            if (item.Interruption != null)
            {
                item.Interruption.ProcessHistoryId = item.Id;
            }

            // Chek or Insert ProductSpecification
            if (item.ProductSpecification != null)
            {
                item.ProductSpecificationCode = item.ProductSpecification.Code;
                item.ProductSpecification = await GetOrAddProductSpecificationDtoAsync(item.ProductSpecification);
            }

            // Update ProcessHistoryDto
            await repository.UpdateAsync(item);

            // Get current interruption
            var interruptionCurrent = await GetInterruptionDtoByProcessAsync(item.Id);

            // Insert interruption
            if (item.Interruption != null && interruptionCurrent == null)
            {
                item.Interruption = await repository.InsertAsync(item.Interruption);
            }
            // Disable/Delete interruption
            else if (interruptionCurrent != null && item.Interruption == null)
            {
                item.Interruption.Enable = false;
                await repository.UpdateAsync<InterruptionHistoryModel>(item.Interruption, new[] { "Enable" });
            }
            // Update interruption
            else if (interruptionCurrent != null && item.Interruption != null)
            {
                await repository.UpdateAsync(item.Interruption);
            }

            return mapper.Map<ProcessHistory>(item);
        }

    }
}
