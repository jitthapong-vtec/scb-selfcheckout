using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels
{
    public class LandingViewModel : ViewModelBase
    {
        public override async Task InitializeAsync(object navigationData)
        {
            await NavigationService.NavigateToAsync<LoginViewModel>();
        }
    }
}
