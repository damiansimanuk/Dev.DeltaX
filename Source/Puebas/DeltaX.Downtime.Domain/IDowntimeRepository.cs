namespace DeltaX.Downtime.Domain
{
    using DeltaX.Downtime.Domain.ProcessAggregate;
    using DeltaX.Domain.Common.Repositories;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDowntimeRepository : IBasicRepository<ProcessHistory, Guid>  
    {
        Task<ProductSpecification> GetProductSpecificationAsync(string code); 
          
        Task<InterruptionHistory> GetInterruptionByProcessAsync(Guid processId); 
    }
}
