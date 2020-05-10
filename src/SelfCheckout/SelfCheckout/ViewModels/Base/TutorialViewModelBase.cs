using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Services.SelfCheckout;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class TutorialViewModelBase : NavigatableViewModelBase
    {
        ObservableCollection<TutorialImage> _assets;

        public TutorialViewModelBase(INavigationService navigationService, ISelfCheckoutService selfCheckoutService) : base(navigationService)
        {
            SelfCheckoutService = selfCheckoutService;
        }

        protected ISelfCheckoutService SelfCheckoutService { get; }

        public ObservableCollection<TutorialImage> Assets
        {
            get => _assets;
            set => SetProperty(ref _assets, value);
        }

        protected async Task LoadImageAsset()
        {
            try
            {
                IsBusy = true;
                var lang = SelfCheckoutService.CurrentLanguage.LangCode;
                await SelfCheckoutService.GetTutorialImageAsync(lang.ToLower());
            }
            catch { }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
