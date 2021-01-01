namespace DeltaX.ApiRestLongPollingTest.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DeltaX.ApiRestLongPollingTest;
    using DeltaX.ApiRestLongPollingTest.Dto;
    using Microsoft.Extensions.Logging;


    public class WeatherRepository : IWeatherRepository 
    {
        private List<WeatherForecast> items; 
        private ILogger<WeatherRepository> logger; 
        
        public WeatherRepository(ILogger<WeatherRepository> logger =null)
        {
            this.logger = logger;
            this.items = new List<WeatherForecast>();
        }

        public IEnumerable<WeatherForecast> InsertMany(IEnumerable<CreateWeatherDto> newItemsDto)
        {
            var newItems = newItemsDto.Select(i => new WeatherForecast
            {
                Id = Guid.NewGuid(),
                Date = i.Date,
                Summary = i.Summary,
                Region = i.Region,
                TemperatureC = i.TemperatureC
            }).ToArray();

            items.AddRange(newItems);
            logger?.LogInformation("InsertMany DB Count:{0}", newItems.Count());
            return newItems;
        }

        public WeatherForecast Insert(CreateWeatherDto newItem)
        {
            var item = new WeatherForecast
            {
                Id = Guid.NewGuid(),
                Date = newItem.Date,
                Summary = newItem.Summary,
                Region = newItem.Region,
                TemperatureC = newItem.TemperatureC
            };
            logger?.LogInformation("Insert DB id:{0}",item.Id);
            items.Add(item); 
            return item;
        }

        public WeatherForecast Update(WeatherForecast item)
        { 
            logger?.LogInformation("Update DB id:{0}",item.Id);
            var prevItem = Remove(item.Id);
            if(prevItem != null)
            {                
                items.Add(item); 
                return item;
            }
            return null;
        }

        public WeatherForecast Remove(Guid itemId)
        { 
            var item = Get(itemId);
            if(item!=null)
            { 
                items.Remove(item);
            }
            return item; 
        }
        
        public WeatherForecast Get(Guid itemId)
        {
            logger?.LogInformation("Get DB itemId:{0}",itemId);
            return items.FirstOrDefault(i=> i.Id == itemId);
        }

        public PaginatingResultDto<WeatherForecast> GetAll(int skipCount = 0, int? maxResultCount = 10)
        {
            var total = items.Count();
            maxResultCount = maxResultCount ?? total;

            var resItems = items.Skip(skipCount).Take(maxResultCount.Value);
            logger?.LogInformation("GetAll DB Count:{0}", resItems.Count());

            return new PaginatingResultDto<WeatherForecast>(total, maxResultCount.Value, skipCount, resItems); 
        }
    }
}

