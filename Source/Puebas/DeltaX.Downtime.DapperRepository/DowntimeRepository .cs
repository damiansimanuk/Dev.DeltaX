﻿using DeltaX.Domain.Common.Repositories;
using DeltaX.Downtime.Domain;
using DeltaX.Downtime.Domain.ProcessAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DeltaX.Repository.DapperRepository;
using Microsoft.Extensions.Logging;
using DeltaX.Downtime.DapperRepository.Dto;
using DeltaX.Utilities;

namespace DeltaX.Downtime.DapperRepository
{
    public class DowntimeRepository :
        IDowntimeRepository,
        IBasicRepository<ProcessHistory, Guid>
    {

        private DowntimeUnitOfWork unitOfWork;
        private DowntimeTableQueryFactory queryFactory;
        private DowntimeRepositoryMapper mapper;
        private ILogger logger;
        private Repository.DapperRepository.DapperRepository repository;


        public DowntimeRepository(
            DowntimeUnitOfWork unitOfWork,
            DowntimeTableQueryFactory queryFactory,
            DowntimeRepositoryMapper mapper,
            ILogger<DowntimeRepository> logger)
        {
            this.unitOfWork = unitOfWork;
            this.queryFactory = queryFactory;
            this.mapper = mapper;
            this.logger = logger;
            this.repository = new DapperRepository<ProcessHistoryDto>(unitOfWork, queryFactory, logger);
        }



        public IUnitOfWork UnitOfWork => unitOfWork;

        public Task DeleteAsync(Guid id)
        {
            return repository.DeleteAsync(new ProcessHistoryDto { Id = id });
        }

        public Task DeleteAsync(ProcessHistory entity)
        {
            return DeleteAsync(entity.Id);
        }


        public Task<ProcessHistory> GetAsync(ProcessHistory entity, bool includeDetails = false)
        { 
            return GetAsync(entity.Id, includeDetails);
        }

        public async Task<ProcessHistory> GetAsync(Guid id, bool includeDetails = false)
        {
            var item = await repository.GetAsync(new ProcessHistoryDto { Id = id });

            item.Interruption = await GetInterruptionDtoByProcessAsync(id);

            if (!string.IsNullOrEmpty(item.ProductSpecificationCode))
            {
                item.ProductSpecification = await GetProductSpecificationDtoAsync(item.ProductSpecificationCode);
            }

            return mapper.Map<ProcessHistory>(item);
        }

        public Task<long> GetCountAsync()
        {
            return repository.GetCountAsync<ProcessHistoryDto>(null);
        }

        private Task<ProductSpecificationDto> GetProductSpecificationDtoAsync(string code)
        {
            return repository.GetAsync<ProductSpecificationDto>(
                @"WHERE Code = @Code",
                new { Code = code });
        }

        private async Task<ProductSpecificationDto> GetOrAddProductSpecificationDtoAsync(ProductSpecificationDto productSpecification)
        {
            // Crea si no existe
            var current = await GetProductSpecificationDtoAsync(productSpecification.Code);
            if (current != null)
            {
                return current;
            }

            return await repository.InsertAsync(productSpecification);
        }

        public async Task<ProductSpecification> GetProductSpecificationAsync(string code)
        {
            var item = await GetProductSpecificationDtoAsync(code);
            return mapper.Map<ProductSpecification>(item);
        }

        private async Task<InterruptionHistoryDto> UpdateInterruptionDtoAsync(InterruptionHistoryDto interruption)
        {
            // Get current interruption
            var current = await GetInterruptionDtoByProcessAsync(interruption.ProcessHistoryId);

            // Disable/Delete interruption
            if (current != null)
            {
                var interruptionDisabled = interruption.Copy();
                interruptionDisabled.Enable = false;
                await repository.UpdateAsync(interruptionDisabled, new[] { "Enable" });
            }

            // Insert interruption
            if (interruption != null  )
            {
                interruption = await repository.InsertAsync(interruption);
            }
            
            return interruption;
        } 
        
        
        private Task<InterruptionHistoryDto> GetInterruptionDtoByProcessAsync(Guid processHistoryId)
        {
            return repository.GetAsync<InterruptionHistoryDto>(
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
            var items = await repository.GetPagedListAsync<ProcessHistoryDto>();
            return mapper.Map<List<ProcessHistory>>(items);
        }

        public async Task<IEnumerable<ProcessHistory>> GetPagedListAsync(int skipCount, int maxResultCount,
            string filter = null, string sorting = null, object param = null,  bool includeDetails = false)
        {
            var items = await repository.GetPagedListAsync<ProcessHistoryDto>(skipCount, maxResultCount);
            return mapper.Map<List<ProcessHistory>>(items);
        }

        public async Task<ProcessHistory> InsertAsync(ProcessHistory entity)
        {
            logger.LogInformation("InsertAsync ProcessHistory {@entity}", entity);

            var item = mapper.Map<ProcessHistoryDto>(entity);

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

            var item = mapper.Map<ProcessHistoryDto>(entity);
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
                await repository.UpdateAsync<InterruptionHistoryDto>(item.Interruption, new[] { "Enable" });
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
