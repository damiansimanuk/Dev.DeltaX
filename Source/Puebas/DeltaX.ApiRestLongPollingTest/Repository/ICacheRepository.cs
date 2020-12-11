using DeltaX.ApiRestLongPollingTest.Dto;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaX.ApiRestLongPollingTest.Repository
{
    public interface ICacheRepository
    {
        IObservableCache<DataTracker<WeatherForecast>, Guid> SharedCache { get; } 
    }
}