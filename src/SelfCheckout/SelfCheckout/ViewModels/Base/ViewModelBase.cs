using SelfCheckout.Models;
using SelfCheckout.Services.Dialog;
using SelfCheckout.Services.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class ViewModelBase : ExtendedBindableObject
    {
        protected readonly ISelfCheckoutService SelfCheckoutService;
        protected readonly ISaleEngineService SaleEngineService;
        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;
        protected readonly IRegisterService RegisterService;

        string _pageTitle;
        bool _isBusy;
        bool _isRefreshing;

        public ViewModelBase()
        {
            SelfCheckoutService = ViewModelLocator.Resolve<ISelfCheckoutService>();
            SaleEngineService = ViewModelLocator.Resolve<ISaleEngineService>();
            RegisterService = ViewModelLocator.Resolve<IRegisterService>();
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
        }

        public ICommand LogoutCommand => new Command(async () =>
        {
            var result = await DialogService.ShowConfirmAsync(AppResources.Logout, AppResources.ConfirmLogout, AppResources.Yes, AppResources.No);
            if (result)
            {
                await SaleEngineService.LogoutAsync();
                await NavigationService.InitializeAsync();
            }
        });

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
            get => SelfCheckoutService.AppConfig;
        }

        public LoginData LoginData
        {
            get => SaleEngineService.LoginData;
        }

        public CustomerData CustomerData
        {
            get => RegisterService.CustomerData;
        }

        public string CurrentShoppingCart
        {
            get => SelfCheckoutService.CurrentShoppingCart;
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

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
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
                if (_isBusy == value)
                    return;

                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public string Version { get => VersionTracking.CurrentVersion; }

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
