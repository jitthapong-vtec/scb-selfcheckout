using SelfCheckout.Resources;
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
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                IsBusy = true;
                
                //await MasterDataService.LoadConfigAsync();

                if (SaleEngineService.LoginData == null)
                    await NavigationService.NavigateToAsync<LoginViewModel>();
                else
                    await NavigationService.NavigateToAsync<BorrowViewModel>();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
