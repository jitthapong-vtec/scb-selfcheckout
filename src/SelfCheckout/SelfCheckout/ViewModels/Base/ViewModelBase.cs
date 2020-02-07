using SelfCheckout.Models;
using SelfCheckout.Services.Configuration;
using SelfCheckout.Services.Dialog;
using SelfCheckout.Services.Navigation;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class ViewModelBase : ExtendedBindableObject
    {
        protected readonly IAppConfigService AppConfigService;
        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;

        string _pageTitle;
        bool _isBusy;

        public ViewModelBase()
        {
            AppConfigService = ViewModelLocator.Resolve<IAppConfigService>();
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
        }

        public virtual Task OnTabSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        public virtual Task OnTabDeSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        public AppConfig AppConfig { 
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

        public double BarcodeWidth
        {
            get => Device.Info.PixelScreenSize.Width;
        }

        public double QRcodeWidth
        {
            get => Device.Info.PixelScreenSize.Width * .50;
        }

        public double QRcodeHeight
        {
            get => Device.Info.PixelScreenSize.Height * .33;
        }

        public double QRcodeHeightRequest
        {
            get => Device.Info.ScaledScreenSize.Height * .33;
        }

        public double BarcodeHeight
        {
            get => Device.Info.PixelScreenSize.Height * .25;
        }

        public double BarcodeHeightRequest
        {
            get => Device.Info.ScaledScreenSize.Height * .15;
        }

        public double ListItemImageHeight
        {
            get
            {
                var height = Device.Info.ScaledScreenSize.Height * .25;
                return height;
            }
        }

        public double DetailImageHeight
        {
            get => Device.Info.ScaledScreenSize.Height * .33;
        }

        public double ScaledScreenWidth
        {
            get => Device.Info.ScaledScreenSize.Width;
        }

        public double ScaledScreenHeight
        {
            get => Device.Info.ScaledScreenSize.Height;
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }

        public virtual Task InitializeAsync<TViewModel, TResult>(object param, TaskCompletionSource<TResult> task)
        {
            return Task.FromResult(false);
        }

        public virtual Task OnPopbackAsync()
        {
            NavigationService.SendPopbackMessage();
            return Task.FromResult(true);
        }
    }
}
