using DeltaX.Downtime.Domain.ProcessAggregate;
using DeltaX.Domain.Common.Entities;
using DeltaX.Domain.Common.Extensions;
using DeltaX.Repository.Common.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaX.Downtime.Repository.Mappers
{
    public class ProcessHistoryDto : IEntityDto<Guid>, ITableDto
    {
        public Guid Id { get; set; }

        public DateTime StartProcessDateTime
        {
            get => StartProcessDateTimeDb.DateTime;
            set => StartProcessDateTimeDb = new DateTimeOffset(value);
        }

        public DateTimeOffset StartProcessDateTimeDb { get; set; }

        public DateTime? FinishProcessDateTime
        {
            get => FinishProcessDateTimeDb?.DateTime;
            set => FinishProcessDateTimeDb = value.HasValue ? new DateTimeOffset(value.Value) : default;
        }
        public DateTimeOffset? FinishProcessDateTimeDb { get; set; }

        public string ProductSpecificationCode { get; set; }

        public ProductSpecificationDto ProductSpecification { get; set; }
         
        public InterruptionHistoryDto Interruption { get; set; } 
    }
}
