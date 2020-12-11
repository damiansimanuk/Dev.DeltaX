namespace DeltaX.ApiRestLongPollingTest.Repository
{ 
    using System;
    using System.Collections.Generic;
    using DeltaX.ApiRestLongPollingTest;
    using DeltaX.ApiRestLongPollingTest.Dto;

    public interface IWeatherRepository
    {
        WeatherForecast Get(Guid itemId);
        PaginatingResultDto<WeatherForecast> GetAll(int skipCount = 0, int? maxResultCount = 10 ); 
        WeatherForecast Insert(CreateWeatherDto item);
        IEnumerable<WeatherForecast> InsertMany(IEnumerable<CreateWeatherDto> newItemsDto);
        WeatherForecast Remove(Guid itemId);
        WeatherForecast Update(WeatherForecast item);
    }
}
 
