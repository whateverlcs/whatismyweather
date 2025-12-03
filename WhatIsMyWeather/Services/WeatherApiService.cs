using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using WhatIsMyWeather.Logging;
using WhatIsMyWeather.Models;

namespace WhatIsMyWeather.Services
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _settings;
        private readonly ILoggingService _logger;

        public WeatherApiService(
            HttpClient httpClient,
            IOptions<WeatherApiSettings> options,
            ILoggingService logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<Weather.Root> GetWeatherAsync(string city)
        {
            try
            {
                var queryParams = HttpUtility.ParseQueryString(string.Empty);
                queryParams["city_name"] = city;
                queryParams["key"] = _settings.Key;

                var url = $"?{queryParams}";
                var response = await _httpClient.GetFromJsonAsync<Weather.Root>(url);

                if (response == null)
                    return new Weather.Root();

                response = IdentifyIconsUrls(response);

                return response!;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetWeatherAsync(string city)");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "GetWeatherAsync(string city)");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "GetWeatherAsync(string city)");
            }

            return new Weather.Root();
        }

        public Weather.Root IdentifyIconsUrls(Weather.Root weatherData)
        {
            if (weatherData.results != null)
            {
                weatherData.results.iconUrl = $"{_settings.IconBaseUrl}{weatherData.results.condition_slug}.svg";

                if (weatherData.results.forecast != null)
                {
                    foreach (var forecast in weatherData.results.forecast)
                    {
                        forecast.iconUrl = $"{_settings.IconBaseUrl}{forecast.condition}.svg";
                    }
                }
            }

            return weatherData;
        }
    }
}