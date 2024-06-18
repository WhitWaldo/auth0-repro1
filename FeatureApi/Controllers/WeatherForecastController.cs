using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services.Models.Weather;

namespace FeatureApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpGet]
    [AllowAnonymous]
    public IEnumerable<WeatherForecast> Get([FromQuery(Name="c")]int count)
    {
        return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    [HttpGet("auth")]
    public IEnumerable<WeatherForecast> GetWithAuth([FromQuery(Name = "c")] int count)
    {
        return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    [HttpGet("statement")]
    [AllowAnonymous]
    public string GetAnonymous()
    {
        return "Cloudy";
    }
}