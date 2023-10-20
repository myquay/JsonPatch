namespace JsonPatchCore.Tests.Api
{
    public class WeatherForecast
    {
        public string? Description { get; set; }

        public DateTime? Time { get; set; }

        public WeatherType? Weather { get; set; }
    }

    public enum WeatherType
    {
        Sunny,
        Cloudy,
        Rainy,
        Snowy
    }
}