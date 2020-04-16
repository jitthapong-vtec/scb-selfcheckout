using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Resources;
using SelfCheckout.Services;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class LandingViewModel : NavigatableViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        public LandingViewModel(INavigationService navigatinService, ISelfCheckoutService selfCheckoutService) : base(navigatinService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await ReloadData();
        }

        private async Task ReloadData()
        {
            try
            {
                IsBusy = true;

                await _selfCheckoutService.LoadConfigAsync();
                await _selfCheckoutService.ValidateMachineAsync(GlobalSettings.Instance.MachineIp);

                await NavigationService.NavigateAsync("LoginView");
            }
            catch (Exception ex)
            {
                var result = await NavigationService.ConfirmAsync("Do you want to go to settings?", ex.Message, AppResources.Yes, AppResources.No);
                if (result)
                {
                    if (Device.RuntimePlatform == Device.UWP)
                        await NavigationService.NavigateAsync("CheckerSettingView");
                    else
                        await NavigationService.NavigateAsync("SettingView");
                }
                else
                {
                    DependencyService.Get<INativeApiService>()?.ExitApp();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
