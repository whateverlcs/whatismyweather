using Caliburn.Micro;
using WhatIsMyWeather.Infrastructure;
using WhatIsMyWeather.ViewModels;

namespace WhatIsMyWeather
{
    public static class ActiveView
    {
        public static ShellViewModel Parent;

        public static async Task OpenItem<T>(params object[] args) where T : IScreen
        {
            var viewModel = (T)DependencyResolver.CreateInstance(typeof(T), args);
            await Parent.ActivateItemAsync(viewModel);
        }
    }
}