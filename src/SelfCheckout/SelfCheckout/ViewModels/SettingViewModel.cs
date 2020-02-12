using SelfCheckout.Models;
using SelfCheckout.Services.Device;
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
