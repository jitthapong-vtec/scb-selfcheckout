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
        int _promptPayTimeout;

        public SettingViewModel(INavigationService navigatinService,
            ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService) :
            base(navigatinService, selfCheckoutService, saleEngineService)
        {
        }

        public int PromptPayTimeout
        {
            get => _promptPayTimeout;
            set => SetProperty(ref _promptPayTimeout, value);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            if (PromptPayTimeout > 0)
                Preferences.Set("promptpay_timeout", PromptPayTimeout);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            PromptPayTimeout = GlobalSettings.Instance.PromptPayTimeout;
        }
    }
}
