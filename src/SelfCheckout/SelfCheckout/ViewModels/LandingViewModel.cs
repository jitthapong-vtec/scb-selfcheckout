using SelfCheckout.Resources;
using SelfCheckout.Services.Identity;
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
        IIdentityService _identityService;

        public LandingViewModel(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                IsBusy = true;
                
                await MasterDataService.LoadMasterData();
                LanguageSelected = MasterDataService.Languages.FirstOrDefault();

                if (_identityService.SessionData == null)
                    await NavigationService.NavigateToAsync<LoginViewModel>();
                else
                    await NavigationService.NavigateToAsync<BorrowViewModel>();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Info, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
