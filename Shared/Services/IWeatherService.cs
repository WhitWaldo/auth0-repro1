using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Services.Models.Weather;

namespace Shared.Services;

public interface IWeatherService
{
    Task<WeatherForecast> GetForecastAsync();
}