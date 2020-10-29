using DeltaX.Domain.Common.Entities;
using DeltaX.Repository.Common.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaX.Downtime.Repository.Mappers
{
    public class InterruptionHistoryDto : IEntityDto<int>, ITableDto
    {
        public int Id { get; set; }

        public string ProcessHistoryIdString { get => ProcessHistoryId.ToString("N"); set => ProcessHistoryId = new Guid(value); }

        public Guid ProcessHistoryId { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public static string TableName => "InterruptionHistory";
         
        public static IDictionary<string, object> GetColumns(InterruptionHistoryDto dto = null, bool withPrimaryKey = true)
        {
            var res = new Dictionary<string, object>();
            if (withPrimaryKey)
            {
                res.Add(nameof(Id), dto?.Id);
            }
            res.Add(nameof(ProcessHistoryId), dto?.ProcessHistoryId);
            res.Add(nameof(StartDateTime), dto?.StartDateTime);
            res.Add(nameof(EndDateTime), dto?.EndDateTime); 
            return res;
        }
    }
}
