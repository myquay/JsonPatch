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
            Remove = DateTime.UtcNow,
            Replace = "ReplaceOld",
            From = 123
        };
        
        patch.ApplyTo(weatherForecast);
        
        return weatherForecast;
    }
}