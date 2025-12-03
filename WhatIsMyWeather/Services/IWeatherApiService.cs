using WhatIsMyWeather.Models;

namespace WhatIsMyWeather.Services
{
    public interface IWeatherApiService
    {
        Task<Weather.Root> GetWeatherAsync(string city);

        Weather.Root IdentifyIconsUrls(Weather.Root weatherData);
    }
}