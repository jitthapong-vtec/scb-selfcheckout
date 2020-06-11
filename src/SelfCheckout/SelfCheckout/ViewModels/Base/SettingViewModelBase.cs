using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Device;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class SettingViewModelBase : NavigatableViewModelBase
    {
        public ISelfCheckoutService SelfCheckoutService { get; private set; }
        public ISaleEngineService SaleEngineService { get; private set; }

        string _selfCheckoutApi;
        string _promptPayApi;
        string _machineNo;
        bool _isEnableLog;

        public ICommand CheckedChangeCommand { get; set; }

        public SettingViewModelBase(INavigationService navigatinService,
            ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService) : base(navigatinService)
        {
            SelfCheckoutService = selfCheckoutService;
            SaleEngineService = saleEngineService;

            CheckedChangeCommand = new DelegateCommand(CheckedChange);
        }

        private void CheckedChange()
        {
            IsEnableLog = !IsEnableLog;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            SelfCheckoutApi = GlobalSettings.Instance.SelfCheckoutApi;
            PromptPayApi = GlobalSettings.Instance.PromptPayApi;

            var loginData = parameters.GetValue<LoginData>("LoginData");
            MachineNo = loginData?.UserInfo?.MachineEnv?.MachineNo;

            IsEnableLog = GlobalSettings.Instance.EnableLog;
        }

        protected override Task GoBackToRootAsync()
        {
            if (!string.IsNullOrEmpty(SelfCheckoutApi))
            {
                if (!SelfCheckoutApi.EndsWith("/"))
                    SelfCheckoutApi += "/";
                Preferences.Set("self_checkout_api", SelfCheckoutApi);
            }
            if (!string.IsNullOrEmpty(PromptPayApi))
            {
                if (!PromptPayApi.EndsWith("/"))
                    PromptPayApi += "/";
                Preferences.Set("promptpay_api", PromptPayApi);
            }

            Preferences.Set("enable_log", IsEnableLog);

            return base.GoBackToRootAsync();
        }

        public bool IsEnableLog {
            get => _isEnableLog;
            set => SetProperty(ref _isEnableLog, value);
        }

        public string PromptPayApi
        {
            get => _promptPayApi;
            set => SetProperty(ref _promptPayApi, value);
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
