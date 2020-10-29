namespace DeltaX.Downtime.DapperRepository.Dto
{
    using DeltaX.Domain.Common.Entities;
    using DeltaX.Repository.Common.Table;
    using System;

    public class ProductSpecificationDto : IEntityDto<int>, ITableDto
    {
        public int Id { get; set; }

        public string Code { get; set; }
        
        public int StandarDuration { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public bool Enable { get; set; }
    }
}
