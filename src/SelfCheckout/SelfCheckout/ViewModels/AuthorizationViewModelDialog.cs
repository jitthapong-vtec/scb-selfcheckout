using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class AuthorizationDialogViewModel : AuthorizationViewModelBase
    {
        TaskCompletionSource<INavigationParameters> _tcs;

        public AuthorizationDialogViewModel(INavigationService navigationService, ISaleEngineService saleEngineService,
            ISelfCheckoutService selfCheckoutService) : base(navigationService, saleEngineService, selfCheckoutService)
        {
        }

        protected override async Task AuthorizeCallback(LoginData loginData)
        {
            await SetResult(true, loginData);
        }

        public ICommand CancelCommand => new DelegateCommand(async () =>
        {
            await SetResult(false, null);
        });

        async Task SetResult(bool isAuthorized, LoginData loginData)
        {
            var parameters = new NavigationParameters()
            {
                {"IsAuthorized", isAuthorized },
                {"LoginData", loginData }
            };
            _tcs?.SetResult(parameters);
            await NavigationService.GoBackAsync();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            _tcs = parameters.GetValue<TaskCompletionSource<INavigationParameters>>("TaskResult");
        }
    }
}
