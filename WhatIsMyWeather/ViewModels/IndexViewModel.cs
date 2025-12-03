using Caliburn.Micro;
using System.Windows;
using WhatIsMyWeather.Logging;
using WhatIsMyWeather.Models;
using WhatIsMyWeather.Services;

namespace WhatIsMyWeather.ViewModels
{
    public class IndexViewModel : Screen
    {
        #region Bindings

        private string _textLoading;

        public string TextLoading
        {
            get { return _textLoading; }
            set
            {
                _textLoading = value;
                NotifyOfPropertyChange(() => TextLoading);
            }
        }

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                NotifyOfPropertyChange(() => Loading);
            }
        }

        private bool _showWeather;

        public bool ShowWeather
        {
            get { return _showWeather; }
            set
            {
                _showWeather = value;
                NotifyOfPropertyChange(() => ShowWeather);
            }
        }

        private string _txtSearchCity;

        public string TxtSearchCity
        {
            get { return _txtSearchCity; }
            set
            {
                _txtSearchCity = value;
                NotifyOfPropertyChange(() => TxtSearchCity);
            }
        }

        private string _txtCity;

        public string TxtCity
        {
            get { return _txtCity; }
            set
            {
                _txtCity = value;
                NotifyOfPropertyChange(() => TxtCity);
            }
        }

        private string _iconWeatherUrl;

        public string IconWeatherUrl
        {
            get { return _iconWeatherUrl; }
            set
            {
                _iconWeatherUrl = value;
                NotifyOfPropertyChange(() => IconWeatherUrl);
            }
        }

        private string _txtTemperature;

        public string TxtTemperature
        {
            get { return _txtTemperature; }
            set
            {
                _txtTemperature = value;
                NotifyOfPropertyChange(() => TxtTemperature);
            }
        }

        private string _txtDescription;

        public string TxtDescription
        {
            get { return _txtDescription; }
            set
            {
                _txtDescription = value;
                NotifyOfPropertyChange(() => TxtDescription);
            }
        }

        private string _txtMaxTemperature;

        public string TxtMaxTemperature
        {
            get { return _txtMaxTemperature; }
            set
            {
                _txtMaxTemperature = value;
                NotifyOfPropertyChange(() => TxtMaxTemperature);
            }
        }

        private string _txtMinTemperature;

        public string TxtMinTemperature
        {
            get { return _txtMinTemperature; }
            set
            {
                _txtMinTemperature = value;
                NotifyOfPropertyChange(() => TxtMinTemperature);
            }
        }

        private string _txtRain;

        public string TxtRain
        {
            get { return _txtRain; }
            set
            {
                _txtRain = value;
                NotifyOfPropertyChange(() => TxtRain);
            }
        }

        private string _txtWindSpeed;

        public string TxtWindSpeed
        {
            get { return _txtWindSpeed; }
            set
            {
                _txtWindSpeed = value;
                NotifyOfPropertyChange(() => TxtWindSpeed);
            }
        }

        private string _txtHumidity;

        public string TxtHumidity
        {
            get { return _txtHumidity; }
            set
            {
                _txtHumidity = value;
                NotifyOfPropertyChange(() => TxtHumidity);
            }
        }

        private string _txtSunset;

        public string TxtSunset
        {
            get { return _txtSunset; }
            set
            {
                _txtSunset = value;
                NotifyOfPropertyChange(() => TxtSunset);
            }
        }

        #endregion Bindings

        public BindableCollection<Weather.Forecast> DailyForecasts { get; private set; } = new BindableCollection<Weather.Forecast>();

        private readonly IWeatherApiService _api;
        private readonly ILoggingService _logger;

        public IndexViewModel(ILoggingService logger, IWeatherApiService api)
        {
            _logger = logger;
            _api = api;
        }

        public void SearchWeather()
        {
            Loading = true;
            ShowWeather = false;
            TextLoading = "SEARCHING WEATHER";

            Task.Run(() => SearchWeatherAsync()).ConfigureAwait(false);
        }

        public async void SearchWeatherAsync()
        {
            try
            {
                Weather.Root weatherData = await _api.GetWeatherAsync(TxtSearchCity).ConfigureAwait(false);

                if (weatherData != null && weatherData.results != null)
                {
                    TxtCity = weatherData.results.city;
                    IconWeatherUrl = weatherData.results.iconUrl;
                    TxtTemperature = $"{weatherData.results.temp}°";
                    TxtDescription = weatherData.results.description;
                    TxtMaxTemperature = $"{weatherData.results.forecast[0].max}°";
                    TxtMinTemperature = $"{weatherData.results.forecast[0].min}°";
                    TxtRain = $"{weatherData.results.rain}mm";
                    TxtWindSpeed = $"{weatherData.results.wind_speedy}";
                    TxtHumidity = $"{weatherData.results.humidity}%";
                    TxtSunset = $"{weatherData.results.sunset}";

                    DailyForecasts.Clear();
                    foreach (var forecast in weatherData.results.forecast)
                    {
                        DailyForecasts.Add(forecast);
                    }

                    ShowWeather = true;
                }
                else
                {
                    MessageBox.Show("It was not possible to find the city. Please try again, if the error persists, contact support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred while searching the weather. Please try again, if the error persists, contact support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError(e, "SearchWeather()");
            }
            finally
            {
                Loading = false;
            }
        }
    }
}