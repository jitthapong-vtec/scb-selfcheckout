using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class HomeViewModel : TutorialViewModelBase
    {
        public Action ShowSystemView;

        public HomeViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService) : base(navigationService, selfCheckoutService)
        {
        }

        public ICommand ShowSystemCommand => new Command(async () =>
        {
            var result = await NavigationService.ShowDialogAsync<INavigationParameters>("AuthorizeDialog", null);

            if (result.GetValue<bool>("IsAuthorized"))
            {
                ShowSystemView?.Invoke();
            }
        });

        public async Task ReloadImageAsset()
        {
            try
            {
                await LoadImageAsset();
                Assets = SelfCheckoutService.TutorialImages?.ToObservableCollection();
            }
            catch { }
        }
    }
}
