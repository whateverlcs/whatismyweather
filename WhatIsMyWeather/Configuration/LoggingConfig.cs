using Serilog;
using Serilog.Events;
using System.IO;

namespace WhatIsMyWeather.Configuration
{
    public static class LoggingConfig
    {
        /// <summary>
        /// Realiza a configuração do Serilog e os arquivos de log que irão existir
        /// </summary>
        public static void ConfigureLogging()
        {
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                Path.Combine(logDirectory, "errors.txt"),
                restrictedToMinimumLevel: LogEventLevel.Error,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();
        }
    }
}