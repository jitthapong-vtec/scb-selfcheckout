using SelfCheckout.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Prism.Commands;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.Services.SaleEngine;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        public const string MessageKey_LoadOrder = "LoadOrder";
        public const string MessageKey_OrderLoaded = "OrderLoaded";
        public const string MessageKey_RefreshSummary = "RefreshSummary";
        public const string MessageKey_RefreshOrderList = "RefreshOrderList";
        public const string MessageKey_CurrencyChanged = "CurrencyChanged";
        public const string MessageKey_ShowOrderDetail = "ShowOrderDetail";
        public const string MessageKey_RequestHWScanner = "RequestHWScanner";
        public const string MessageKey_LanguageChanged = "LanguageChanged";
        public const string MessageKey_Logout = "Logout";

        public INavigationService NavigationService { get; private set; }
        public IDialogService DialogService { get; private set; }

        string _pageTitle;
        bool _isBusy;
        bool _isRefreshing;

        public ICommand BackCommand => new DelegateCommand(async () =>
        {
            await NavigationService.GoBackAsync();
        });

        public ViewModelBase(INavigationService navigatinService, IDialogService dialogService)
        {
            NavigationService = navigatinService;
            DialogService = dialogService;
        }

        public virtual Task OnTabSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        public virtual Task OnTabDeSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public bool IsShowArticleImage
        {
            get
            {
                return (App.Current.Container.Resolve(typeof(ISelfCheckoutService)) as ISelfCheckoutService).AppConfig.ShowArticleImage;
            }
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

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual void Destroy()
        {
        }
    }
}
