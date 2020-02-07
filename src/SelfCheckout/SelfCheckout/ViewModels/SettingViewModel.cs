using SelfCheckout.Models;
using SelfCheckout.Services.Device;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        public string DeviceUID
        {
            get => DependencyService.Get<IDeviceInformation>().GetDeviceCode();
        }
    }
}
