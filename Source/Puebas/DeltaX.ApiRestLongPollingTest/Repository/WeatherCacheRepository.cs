namespace DeltaX.ApiRestLongPollingTest.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks; 
    using DeltaX.ApiRestLongPollingTest.Dto;
    using DynamicData;
    using Microsoft.Extensions.Logging;

    public class WeatherCacheRepository : WeatherRepository, IWeatherCacheRepository, IWeatherRepository
    { 
        private SourceCache<WeatherForecast, Guid> sourceCache;
        private readonly ILogger<WeatherRepository> logger;

        public WeatherCacheRepository(
            ILogger<WeatherRepository> logger = null,
            TimeSpan? expireAfter = null,
            int? limitSizeTo = null)
            : base(logger)
        {

            this.logger = logger;
            this.sourceCache = new SourceCache<WeatherForecast, Guid>(w => w.Id);

            // Expiration time
            if (expireAfter.HasValue)
            {
                this.sourceCache.ExpireAfter(u => expireAfter.Value, TimeSpan.FromSeconds(expireAfter.Value.TotalSeconds / 10))
                    .Subscribe(x => logger.LogInformation("SourceCache Expiration: {0} filled trades have been removed from memory", x.Count()));
            }
            // Limit size
            if (limitSizeTo.HasValue)
            {
                this.sourceCache.LimitSizeTo(limitSizeTo.Value)
                    .Subscribe(x => logger.LogInformation("SourceCache LimitSize: {0} filled trades have been removed from memory", x.Count()));
            }

            // Shared object
            this.SharedCache = sourceCache.Connect()
                .Transform(w => new DataTracker<WeatherForecast>(w, null))
                .AsObservableCache();

            // Initialize Cache with all items
            this.sourceCache.AddOrUpdate(base.GetAll(0, limitSizeTo).Items);
        }

        public IObservableCache<DataTracker<WeatherForecast>, Guid> SharedCache { get; private set; }

        public new WeatherForecast Insert(CreateWeatherDto newItem)
        {
            var item = base.Insert(newItem);
            sourceCache.AddOrUpdate(item);
            return item;
        }

        public new IEnumerable<WeatherForecast> InsertMany(IEnumerable<CreateWeatherDto> newItemsDto)
        {
            var items = base.InsertMany(newItemsDto);
            sourceCache.AddOrUpdate(items);
            return items;
        }

        public new WeatherForecast Update(WeatherForecast item)
        {
            item = base.Update(item);
            sourceCache.AddOrUpdate(item);
            return item;
        }

        public new WeatherForecast Remove(Guid itemId)
        {
            var item = base.Remove(itemId);
            if (item != null)
            {
                sourceCache.RemoveKey(item.Id);
            }
            return item;
        }
    }
}

