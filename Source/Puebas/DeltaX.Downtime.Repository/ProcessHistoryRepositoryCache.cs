using DeltaX.Downtime.Domain;
using DeltaX.Downtime.Domain.ProcessAggregate; 
using DeltaX.Downtime.Repository.Mappers;
using DeltaX.Domain.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaX.Downtime.Repository
{
    public class ProcessHistoryRepositoryCache : IProcessHistoryRepository
    {
        private DowntimeRepositoryMapper mapper;
        private List<ProcessHistoryDto> cache = new List<ProcessHistoryDto>();
        private List<ProductSpecificationDto> cacheSpec = new List<ProductSpecificationDto>(); 
        

        public ProcessHistoryRepositoryCache(  DowntimeRepositoryMapper mapper)
        { 
            this.mapper = mapper;
        } 

        public IUnitOfWork UnitOfWork { get; private set; }

        public async Task DeleteAsync(Guid id )
        {
            var item = await GetAsync(id, false);
            await DeleteAsync(item);
        }

        public Task DeleteAsync(ProcessHistory entity )
        {
            cache.RemoveAll(i => i.Id == entity.Id);
            return Task.CompletedTask;
        }

        public Task<ProcessHistory> GetAsync(Guid id, bool includeDetails = false )
        {
            var item = cache.First(e => e.Id == id);

            return Task.FromResult(mapper.Map<ProcessHistory>(item));
        }

        public Task<long> GetCountAsync()
        {
            return Task.FromResult((long)cache.Count());
        }

        public Task<InterruptionHistory> GetInterruptionHistoryAsync(int id )
        {
            var item = cache.First(e => e.Interruption.Id == id).Interruption;

            return Task.FromResult(mapper.Map<InterruptionHistory>(item));
        }

        public Task<List<ProcessHistory>> GetListAsync(bool includeDetails = false )
        { 
            return Task.FromResult(mapper.Map<List<ProcessHistory>>(cache));
        }

        public Task<List<ProcessHistory>> GetPagedListAsync(int skipCount, int maxResultCount, string filter, string sorting, bool includeDetails = false )
        {
            var query = cache.Skip(skipCount).Take(maxResultCount);
            return Task.FromResult(mapper.Map<List<ProcessHistory>>(query));
        }

        public Task<ProductSpecification> GetProductSpecificationAsync(string code )
        {
            var item = cacheSpec.First(s => s.Code == code);

            return Task.FromResult(mapper.Map<ProductSpecification>(item));
        }

        public Task<ProcessHistory> InsertAsync(ProcessHistory entity )
        {
            var item = mapper.Map<ProcessHistoryDto>(entity);
             
            cache.Add(item);

            return Task.FromResult(mapper.Map<ProcessHistory>(item));
        }

        public Task<ProcessHistory> UpdateAsync(ProcessHistory entity )
        {
            var item = cache.First(e => e.Id == entity.Id);
            var newItem = mapper.Map<ProcessHistoryDto>(entity);

            // replace item
            var idx = cache.IndexOf(item);
            cache.RemoveAt(idx);
            cache.Insert(idx, newItem);

            return Task.FromResult(mapper.Map<ProcessHistory>(newItem));
        }
    }
}
