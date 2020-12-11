using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeltaX.ApiRestLongPollingTest.Dto;
using DeltaX.ApiRestLongPollingTest.Repository;
using DynamicData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DeltaX.ApiRestLongPollingTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherCacheRepository repository;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(WeatherCacheRepository repository, ILogger<WeatherForecastController> logger)
        {
            this.repository = repository;
            _logger = logger;
        }

        [HttpGet("Items")]
        public PaginatingResultDto<WeatherForecast> GetAll(
            [FromQuery] int skipCount = 0,
            [FromQuery] int? maxResultCount = 10)
        {
            return repository.GetAll(skipCount, maxResultCount);
        }

        [HttpGet("Item/{id}")]
        public WeatherForecast Get(Guid id)
        {
            return repository.Get(id);
        }

        [HttpPut("Item/{id}")]
        public WeatherForecast Update(Guid id, WeatherForecast item)
        {
            if (id != item.Id)
            {
                throw new ArgumentException("Invalid Id", nameof(id));
            }
            return repository.Update(item);
        }

        [HttpPost("Item")]
        public WeatherForecast InsertSingle(
            [FromBody] CreateWeatherDto newItemDto)
        {
            return repository.Insert(newItemDto);
        }

        [HttpPost("Items")]
        public IEnumerable<WeatherForecast> InsertMany(
            [FromBody] IEnumerable<CreateWeatherDto> newItemsDto)
        {
            return repository.InsertMany(newItemsDto);
        }

        [HttpDelete("Item/{id}")]
        public WeatherForecast Delete(Guid id)
        {
            return repository.Remove(id);
        }


        [HttpPost("Items/GetSince")]
        public async Task<DataTrackerResultDto<WeatherForecast>> GetSince(
            CancellationToken cancellation,
            [FromBody] GetWeatherSinceDto getSince 
            )
        { 
            var since = getSince.Since ?? new DateTimeOffset(DateTime.Now); 

            var res = await repository.SharedCache.Connect()
                .Filter(i => i.Updated > since)
                .GetItemsAsync(
                    TimeSpan.FromSeconds(getSince.Timeout),
                    cancellation,
                    u => u.Count > u.Removes);

            return new DataTrackerResultDto<WeatherForecast>(res); 
        }
    }
}
