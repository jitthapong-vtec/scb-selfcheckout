using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Device;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        public SettingViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            Preferences.Set("self_checkout_api", SelfCheckoutApi);
        }

        public string SelfCheckoutApi { get => GlobalSettings.Instance.SelfCheckoutApi; }

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
