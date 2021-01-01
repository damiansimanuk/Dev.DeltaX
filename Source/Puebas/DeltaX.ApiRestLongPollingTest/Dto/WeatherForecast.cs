using System;

namespace DeltaX.ApiRestLongPollingTest.Dto
{
    public class WeatherForecast
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public string Region { get; set; }
    }
}
