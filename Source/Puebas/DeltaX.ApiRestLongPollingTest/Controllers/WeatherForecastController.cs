using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
        private readonly IWeatherCacheRepository cacheRepository;
        private readonly IWeatherRepository repository;
        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(
            IWeatherRepository repository, 
            IWeatherCacheRepository cacheRepository, 
            ILogger<WeatherForecastController> logger)
        {
            this.repository = repository;
            this.cacheRepository = cacheRepository;
            this.logger = logger;
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
        public   Task<DataTrackerResultDto<WeatherForecast>> GetSince(
            CancellationToken cancellation,
            [FromBody] GetWeatherSinceDto getSince 
            )
        { 
            var since = getSince.Since ?? new DateTimeOffset(DateTime.Now);
            var region = getSince.Region;
            var timeout = TimeSpan.FromSeconds(getSince.Timeout);

            bool filter(DataTracker<WeatherForecast> i) =>
                !string.IsNullOrEmpty(region)
                ? string.Compare(region, i.Item.Region, true) == 0 && i.Updated > since
                : i.Updated > since;

            return cacheRepository.GetItemsAsync(filter, timeout, cancellation); 
        }

        [HttpGet("Items/GetRemoved")]
        public async Task<DataTrackerResultDto<WeatherForecast>> GetRemoved(
            CancellationToken cancellation,
            [FromQuery] int timeout = 200
            )
        {
            return await cacheRepository.GetRemoved(TimeSpan.FromSeconds(timeout), cancellation);

            // var since = new DateTimeOffset(DateTime.Now);
            // var res = await cacheRepository.SharedCache.Connect()
            //     .WhereReasonsAre(ChangeReason.Remove)
            //     .Filter(i => i.Updated > since)
            //     .GetItemsAsync(
            //         TimeSpan.FromSeconds(timeout),
            //         cancellation,
            //         u => true);
            // 
            // return new DataTrackerResultDto<WeatherForecast>(res);
        }
    }
}
