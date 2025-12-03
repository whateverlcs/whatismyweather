using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.IO;
using System.Net.Http;
using System.Windows;
using WhatIsMyWeather.Configuration;
using WhatIsMyWeather.Infrastructure;
using WhatIsMyWeather.Logging;
using WhatIsMyWeather.Models;
using WhatIsMyWeather.Services;
using WhatIsMyWeather.ViewModels;

namespace WhatIsMyWeather
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();
        private IServiceProvider _serviceProvider;

        public Bootstrapper()
        {
            LoggingConfig.ConfigureLogging();
            Initialize();
        }

        protected override void Configure()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);

            services.Configure<WeatherApiSettings>(configuration.GetSection("WeatherApi"));

            services.AddSingleton<ILoggingService, SerilogLogger>();

            // Configuração HttpClient com Polly
            services.AddHttpClient<IWeatherApiService, WeatherApiService>((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<WeatherApiSettings>>().Value;

                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
                client.DefaultRequestHeaders.Add("User-Agent", "WhatIsMyWeather/1.0");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<IEventAggregator, EventAggregator>();

            services.AddTransient<ShellViewModel>();
            services.AddTransient<IndexViewModel>();

            _serviceProvider = services.BuildServiceProvider();

            _container.Instance(_serviceProvider);
            _container.Instance(_serviceProvider.GetService<IWindowManager>());
            _container.Instance(_serviceProvider.GetService<IEventAggregator>());

            var loggingService = _serviceProvider.GetService<ILoggingService>();
            _container.Instance<ILoggingService>(loggingService);

            var weatherService = _serviceProvider.GetService<IWeatherApiService>();
            _container.Instance<IWeatherApiService>(weatherService);

            // 8. Registrar ViewModels no SimpleContainer
            _container.PerRequest<ShellViewModel>();
            _container.PerRequest<IndexViewModel>();

            DependencyResolver.SetContainer(_container);
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30));
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null) return instance;

            instance = _serviceProvider.GetService(service);
            if (instance != null) return instance;

            throw new InvalidOperationException($"Não pode resolver {service.Name}");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            var simpleInstances = _container.GetAllInstances(service);
            var diInstances = _serviceProvider.GetServices(service);

            var allInstances = new List<object>();
            allInstances.AddRange(simpleInstances);
            allInstances.AddRange(diInstances);

            return allInstances;
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}