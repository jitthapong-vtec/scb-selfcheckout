using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Device;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class SettingViewModelBase : ViewModelBase
    {
        public ISelfCheckoutService SelfCheckoutService { get; private set; }
        public ISaleEngineService SaleEngineService { get; private set; }

        string _selfCheckoutApi;
        string _machineNo;

        public SettingViewModelBase(INavigationService navigatinService, IDialogService dialogService,
            ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService) : base(navigatinService, dialogService)
        {
            SelfCheckoutService = selfCheckoutService;
            SaleEngineService = saleEngineService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            SelfCheckoutApi = GlobalSettings.Instance.SelfCheckoutApi;

            var loginData = parameters.GetValue<LoginData>("LoginData");
            MachineNo = loginData?.UserInfo?.MachineEnv?.MachineNo;
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
            get => SelfCheckoutService.AppConfig;
        }

        public string DeviceUID
        {
            get => DependencyService.Get<IDeviceInformation>().GetDeviceCode();
        }

        public string MachineNo
        {
            get => _machineNo;
            set => SetProperty(ref _machineNo, value);
        }
    }
}
