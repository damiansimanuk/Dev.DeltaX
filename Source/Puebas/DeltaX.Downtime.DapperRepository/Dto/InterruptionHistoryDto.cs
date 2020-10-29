namespace DeltaX.Downtime.DapperRepository.Dto
{
    using DeltaX.Domain.Common.Entities;
    using DeltaX.Repository.Common.Table;
    using System; 

    public class InterruptionHistoryDto : IEntityDto<int>, ITableDto
    {
        public int Id { get; set; }
          
        public Guid ProcessHistoryId { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Enable { get; set; }
    }
}
