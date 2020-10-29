using DeltaX.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaX.Downtime.Repository.Mappers
{
    public static class TablesConfigurationExtension
    { 
        public static IDictionary<string, object> GetColumns<TEntity>(this TEntity entityDto, bool withPrimaryKey = true) where TEntity: IEntityDto
        {
            var res = new Dictionary<string, object>();

            if (typeof(TEntity) == typeof(ProcessHistoryDto))
            {
                var dto = entityDto as ProcessHistoryDto;
                res.Add(nameof(ProcessHistoryDto.Id), dto?.Id);
                res.Add(nameof(ProcessHistoryDto.StartProcessDateTime), dto?.StartProcessDateTime);
                res.Add(nameof(ProcessHistoryDto.FinishProcessDateTime), dto?.FinishProcessDateTime);
                res.Add(nameof(ProcessHistoryDto.ProductSpecificationCode), dto?.ProductSpecificationCode);
            }

            return res;
        }

        public static  string GetTableName<TEntity>(this TEntity entityDto) where TEntity : IEntityDto
        {
            var entityType = typeof(TEntity);
            if (entityType == typeof(ProcessHistoryDto))
            {
                return "ProcessHistory";
            }
            if (entityType == typeof(InterruptionHistoryDto))
            {
                return "InterruptionHistory";
            }
            if (entityType == typeof(ProductSpecificationDto))
            {
                return "ProductSpecification";
            } 

            throw new ArgumentOutOfRangeException("Table Name not configurated!");
        }
    }
}
