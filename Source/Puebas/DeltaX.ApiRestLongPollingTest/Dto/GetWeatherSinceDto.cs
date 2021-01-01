namespace DeltaX.ApiRestLongPollingTest.Dto
{
    using System;

    public class GetWeatherSinceDto
    {
        public DateTimeOffset? Since { get; set; }
        public int Timeout { get; set; } = 60;
        public string Region { get; set; }
    }
}
