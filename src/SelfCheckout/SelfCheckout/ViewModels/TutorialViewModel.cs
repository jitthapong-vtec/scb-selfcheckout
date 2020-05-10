using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
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

        public TutorialViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService) : base(navigationService, selfCheckoutService)
        {
            RefreshLanguage();
        }

        public void RefreshLanguage()
        {
            LabelHelp = AppResources.Help;
        }

        public ICommand HideTutorialCommand => new Command(() => TutorialViewVisible = false);

        public string LabelHelp
        {
            get => _labelHelp;
            set => SetProperty(ref _labelHelp, value);
        }

        public bool TutorialViewVisible
        {
            get => _tutorialViewVisible;
            set => SetProperty(ref _tutorialViewVisible, value);
        }

        public async Task ShowTutorialAsync()
        {
            TutorialViewVisible = true;
            await ReloadImageAsset();
        }

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
