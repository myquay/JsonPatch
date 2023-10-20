using JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace JsonPatchCore.Tests.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpPatch]
    public WeatherForecast Patch(JsonPatchDocument<WeatherForecast> patch)
    {
        var weatherForecast = new WeatherForecast
        {
            Time = DateTime.UtcNow,
            Description = "Sunny",
            Weather = WeatherType.Sunny
        };
        
        patch.ApplyTo(weatherForecast);
        
        return weatherForecast;
    }
}