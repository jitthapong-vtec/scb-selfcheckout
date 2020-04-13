using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Services.PimCore;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class TutorialViewModel : TutorialViewModelBase
    {
        bool _tutorialViewVisible;

        public TutorialViewModel(IDialogService dialogService, ISelfCheckoutService selfCheckoutService,
            IPimCoreService pimCoreService) : base(dialogService, selfCheckoutService, pimCoreService)
        {
        }

        public ICommand HideTutorialCommand => new Command(() => TutorialViewVisible = false);

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
    }
}
