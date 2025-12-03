namespace WhatIsMyWeather.Logging
{
    public interface ILoggingService
    {
        public void LogError(Exception ex, string localException);
    }
}