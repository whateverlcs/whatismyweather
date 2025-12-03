using Caliburn.Micro;

namespace WhatIsMyWeather.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel()
        {
            ActiveView.Parent = this;
        }

        protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
        {
            await base.OnActivatedAsync(cancellationToken);

            await ActiveView.OpenItem<IndexViewModel>();
        }
    }
}