using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels
{
    public class LandingViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        public LandingViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            await ReloadData();
        }

        private async Task ReloadData()
        {
            try
            {
                IsBusy = true;

                await _selfCheckoutService.LoadConfigAsync();
                await NavigationService.NavigateAsync("LoginView");
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
                await ReloadData();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
