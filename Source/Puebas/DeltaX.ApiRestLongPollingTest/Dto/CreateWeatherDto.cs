namespace DeltaX.ApiRestLongPollingTest.Dto
{
    using System;

    public class CreateWeatherDto
    { 
        public DateTimeOffset Date { get; set; } 

        public int TemperatureC { get; set; } 

        public string Summary { get; set; }

        public string Region { get; set; }
    }
}
