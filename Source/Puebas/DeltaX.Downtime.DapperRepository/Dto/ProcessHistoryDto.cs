﻿namespace DeltaX.Downtime.DapperRepository.Dto
{
    using DeltaX.Domain.Common.Entities;
    using DeltaX.Repository.Common.Table;
    using System;

    public class ProcessHistoryDto : IEntity<Guid>, ITableDto
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
        
        public DateTime CreatedAt { get; set; }
        
        public bool Enable { get; set; }

    }
}
