using SelfCheckout.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        public INavigationService NavigationService { get; private set; }
        public IDialogService DialogService { get; private set; }
        public ISaleEngineService SaleEngineService { get; private set; }
        public ISelfCheckoutService SelfCheckoutService { get; private set; }
        public IRegisterService RegisterService { get; set; }

        string _pageTitle;
        bool _isBusy;
        bool _isRefreshing;

        public ICommand LogoutCommand => new Command(async () =>
        {
            //var result = await DialogService.ShowConfirmAsync(AppResources.Logout, AppResources.ConfirmLogout, AppResources.Yes, AppResources.No);
            //if (result)
            //{
            //    await SaleEngineService.LogoutAsync();
            //    await NavigationService.InitializeAsync();
            //}
        });

        public ICommand BackCommand => new Command(async () =>
        {
            await NavigationService.GoBackAsync();
        });

        public ViewModelBase(INavigationService navigatinService, IDialogService dialogService,
            ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService)
        {
            NavigationService = navigatinService;
            DialogService = dialogService;
            SelfCheckoutService = selfCheckoutService;
            SaleEngineService = saleEngineService;
            RegisterService = registerService;
        }

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
            set => SetProperty(ref _pageTitle, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public bool IsBusy
        {
            get => _isBusy;

            set => SetProperty(ref _isBusy, value);
        }

        public string Version { get => VersionTracking.CurrentVersion; }

        public virtual async void Initialize(INavigationParameters parameters)
        {
        }

        public virtual async void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual async void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public void Destroy()
        {
        }
    }
}
