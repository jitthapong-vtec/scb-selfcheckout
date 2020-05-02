using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.PimCore;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class TutorialViewModel : TutorialViewModelBase
    {
        bool _tutorialViewVisible;
        string _labelHelp;

        public TutorialViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService,
            IPimCoreService pimCoreService) : base(navigationService, selfCheckoutService, pimCoreService)
        {
            RefreshLanguage();
        }

        public void RefreshLanguage()
        {
            LableHelp = AppResources.Help;
        }

        public ICommand HideTutorialCommand => new Command(() => TutorialViewVisible = false);

        public string LableHelp
        {
            get => _labelHelp;
            set => SetProperty(ref _labelHelp, value);
        }

        public bool TutorialViewVisible
        {
            get => _tutorialViewVisible;
            set => SetProperty(ref _tutorialViewVisible, value);
        }

        public void ShowTutorial()
        {
            Assets = PimCoreService.ImageAssets?.ToObservableCollection();
            TutorialViewVisible = true;
        }

        public async Task ReloadImageAsset()
        {
            try
            {
                if (Assets.Any())
                    return;
                await LoadImageAsset();
            }
            catch { }
        }
    }
}
