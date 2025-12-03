namespace WhatIsMyWeather.Models
{
    public class WeatherApiSettings
    {
        public string Key { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.hgbrasil.com/weather";
        public int TimeoutSeconds { get; set; } = 30;
        public string IconBaseUrl { get; set; } = "https://assets.hgbrasil.com/weather/icons/conditions/";
    }
}