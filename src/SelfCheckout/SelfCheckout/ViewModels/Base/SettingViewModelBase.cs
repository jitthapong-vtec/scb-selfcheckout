using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Device;
using SelfCheckout.Services.SelfCheckout;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class SettingViewModelBase : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        string _selfCheckoutApi;

        public SettingViewModelBase(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            SelfCheckoutApi = GlobalSettings.Instance.SelfCheckoutApi;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            if (!string.IsNullOrEmpty(SelfCheckoutApi))
                Preferences.Set("self_checkout_api", SelfCheckoutApi);
        }

        public string SelfCheckoutApi
        {
            get => _selfCheckoutApi;
            set => SetProperty(ref _selfCheckoutApi, value);
        }

        public AppConfig AppConfig
        {
            get => _selfCheckoutService.AppConfig;
        }

        public string DeviceUID
        {
            get => DependencyService.Get<IDeviceInformation>().GetDeviceCode();
        }
    }
}
