using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Services.PimCore;
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

        public HomeViewModel(IDialogService dialogService, ISelfCheckoutService selfCheckoutService,
            IPimCoreService pimCoreService) : base(dialogService, selfCheckoutService, pimCoreService)
        {
        }

        public ICommand ShowSystemCommand => new Command(() =>
        {
            DialogService.ShowDialog("AuthorizeDialog", null, (result) =>
            {
                if (result.Parameters.GetValue<bool>("IsAuthorized"))
                {
                    ShowSystemView?.Invoke();
                }
            });
        });

        public override async Task OnTabSelected(TabItem item)
        {
            await LoadImageAsset();
            Assets = PimCoreService.ImageAssets?.ToObservableCollection();
        }
    }
}
