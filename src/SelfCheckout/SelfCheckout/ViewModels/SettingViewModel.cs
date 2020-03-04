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
        public SettingViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public void SaveSettings()
        {
            Preferences.Set("self_checkout_api", SelfCheckoutApi);
        }

        public string SelfCheckoutApi { get => GlobalSettings.Instance.SelfCheckoutApi; }

        public string DeviceUID
        {
            get => DependencyService.Get<IDeviceInformation>().GetDeviceCode();
        }
    }
}
