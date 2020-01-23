using SelfCheckout.Services.Identity;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
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
            if (_identityService.LoginData == null)
                await NavigationService.NavigateToAsync<LoginViewModel>();
            else
                await NavigationService.NavigateToAsync<BorrowViewModel>();
        }
    }
}
