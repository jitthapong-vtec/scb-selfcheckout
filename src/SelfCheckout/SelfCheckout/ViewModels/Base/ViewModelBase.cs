using SelfCheckout.Models;
using SelfCheckout.Services.Master;
using SelfCheckout.Services.Dialog;
using SelfCheckout.Services.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class ViewModelBase : ExtendedBindableObject
    {
        protected readonly IMasterDataService AppConfigService;
        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;

        string _pageTitle;
        bool _isBusy;

        public ViewModelBase()
        {
            AppConfigService = ViewModelLocator.Resolve<IMasterDataService>();
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
        }

        public ICommand BackCommand => new Command(async () =>
        {
            await NavigationService.PopBackAsync();
        });

        public virtual Task OnTabSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        public virtual Task OnTabDeSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        public AppConfig AppConfig
        {
            get => AppConfigService.AppConfig;
        }

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }

        public virtual Task InitializeAsync<TViewModel, TResult>(object param, TaskCompletionSource<TResult> task)
        {
            return Task.FromResult(false);
        }

        public virtual Task NavigationPushed()
        {
            return Task.FromResult(false);
        }

        public virtual Task NavigationPoped()
        {
            return Task.FromResult(false);
        }
    }
}
