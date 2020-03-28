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
    public class SettingViewModel : SettingViewModelBase
    {
        bool _isTestMode;

        public SettingViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService, selfCheckoutService)
        {
            IsTestMode = GlobalSettings.Instance.IsTestMode;
        }

        public bool IsTestMode
        {
            get => _isTestMode;
            set => SetProperty(ref _isTestMode, value, () => Preferences.Set("IsTestMode", value));
        }
    }
}
