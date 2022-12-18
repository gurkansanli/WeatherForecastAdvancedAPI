using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WeatherForecast.Api.Models;

namespace WeatherForecast.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        List<WeatherForecast> WeatherForecasts { get; set; }

        public WeatherForecastController()
        {
            WeatherForecasts = new List<WeatherForecast>();
            WeatherForecasts.AddRange(Enumerable.Range(1, 2).Select(index => new WeatherForecast
            {
                Id = index,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = 5,
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                TemperatureF = 32 + (int)(5 / 0.5556),
            }));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            ResponseBase<WeatherForecast> response = new();
            try
            {
                var weatherForecast = WeatherForecasts.Where(k => k.Id == id).Select(k => new WeatherForecast
                {
                    Id = k.Id,
                    Date = k.Date,
                    Summary = k.Summary,
                    TemperatureC = k.TemperatureC,
                    TemperatureF = k.TemperatureF,
                }).FirstOrDefault();

                response.Item = weatherForecast;

                return await Task.FromResult(StatusCode(response.Status, response));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("MultipleGetWeatherForecast")]
        public async Task<IActionResult> MultipleGet([FromForm] WeatherForecastFilter filter)
        {
            ResponseBase<List<WeatherForecast>> response = new();
            try
            {
                var weatherForecasts = WeatherForecasts.Where(k => (filter.Search == null || k.Summary == filter.Search)
                && (filter.Summary == null || filter.Summary == k.Summary)).Select(k => new WeatherForecast
                {
                    Id = k.Id,
                    Date = k.Date,
                    Summary = k.Summary,
                    TemperatureC = k.TemperatureC,
                    TemperatureF = k.TemperatureF,
                }).ToList();

                response.Item = weatherForecasts;
                response.Count = weatherForecasts.Count;

                return await Task.FromResult(StatusCode(response.Status, response));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("AddWeatherForecast")]
        public async Task<IActionResult> Add([FromForm] WeatherForecast form)
        {
            ResponseBase<WeatherForecast> response = new();
            try
            {
                #region Validations
                if (string.IsNullOrEmpty(form.Summary))
                {
                    response.Status = 400;
                    response.StatusTexts.Add("Summary cannot be empty!");
                }
                if (!form.TemperatureC.HasValue)
                {
                    response.Status = 400;
                    response.StatusTexts.Add("Temperature cannot be empty!");
                }
                if (!form.Date.HasValue)
                {
                    response.Status = 400;
                    response.StatusTexts.Add("Date cannot be empty!");
                }
                else
                {
                    var isWeatherExist = WeatherForecasts.Any(k => k.Date?.Date == form.Date?.Date);

                    if (isWeatherExist)
                    {
                        response.Status = 400;
                        response.StatusTexts.Add("Given date already has weather forecast!");
                    }
                }
                if (response.Status != 200)
                {
                    return await Task.FromResult(StatusCode(response.Status, response));
                }
                #endregion

                WeatherForecast weatherForecast = new()
                {
                    TemperatureC = form.TemperatureC,
                    Date = form.Date,
                    TemperatureF = form.TemperatureC.HasValue ? 32 + (int)(form.TemperatureC / 0.5556) : null,
                    Summary = form.Summary,
                };

                WeatherForecasts.Add(weatherForecast);

                return await Task.FromResult(StatusCode(response.Status, response));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateWeatherForecast")]
        public async Task<IActionResult> Update([FromForm] WeatherForecast form)
        {
            ResponseBase<WeatherForecast> response = new();
            try
            {
                var weatherForecast = WeatherForecasts.FirstOrDefault(k => k.Id == form.Id);

                #region Validations
                if (weatherForecast == null)
                {
                    response.Status = 400;
                    response.StatusTexts.Add("Weather forecast not found!");
                    return await Task.FromResult(StatusCode(response.Status, response));
                }
                if (string.IsNullOrEmpty(form.Summary))
                {
                    response.Status = 400;
                    response.StatusTexts.Add("Summary cannot be empty!");
                }
                if (!form.TemperatureC.HasValue)
                {
                    response.Status = 400;
                    response.StatusTexts.Add("Temperature cannot be empty!");
                }
                if (!form.Date.HasValue)
                {
                    response.Status = 400;
                    response.StatusTexts.Add("Date cannot be empty!");
                }
                else
                {
                    var isWeatherExist = WeatherForecasts.Any(k => k.Id != form.Id && k.Date?.Date == form.Date?.Date);

                    if (isWeatherExist)
                    {
                        response.Status = 400;
                        response.StatusTexts.Add("Given date already has weather forecast!");
                    }
                }
                if (response.Status != 200)
                {
                    return await Task.FromResult(StatusCode(response.Status, response));
                }
                #endregion

                weatherForecast.TemperatureC = form.TemperatureC;
                weatherForecast.Date = form.Date;
                weatherForecast.TemperatureF = form.TemperatureC.HasValue ? 32 + (int)(form.TemperatureC / 0.5556) : null;
                weatherForecast.Summary = form.Summary;

                return await Task.FromResult(StatusCode(response.Status, response));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}