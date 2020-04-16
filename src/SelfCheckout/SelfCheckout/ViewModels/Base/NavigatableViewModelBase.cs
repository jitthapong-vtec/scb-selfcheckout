using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class NavigatableViewModelBase : ViewModelBase, INavigationAware
    {
        public NavigatableViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public INavigationService NavigationService { get; private set; }

        public ICommand BackCommand => new DelegateCommand(async () =>
        {
            await GoBackAsync();
        });

        protected async Task GoBackAsync()
        {
            await NavigationService.GoBackAsync();
        }

        protected async Task GoBackToRootAsync()
        {
            await NavigationService.GoBackToRootAsync();
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
