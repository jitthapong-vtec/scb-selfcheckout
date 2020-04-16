﻿using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Services.PimCore;
using SelfCheckout.Services.SelfCheckout;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class TutorialViewModelBase : NavigatableViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        ObservableCollection<PimCoreImageAsset> _assets;

        public TutorialViewModelBase(INavigationService navigationService, ISelfCheckoutService selfCheckoutService, 
            IPimCoreService pimCoreService) : base(navigationService)
        {
            _selfCheckoutService = selfCheckoutService;
            PimCoreService = pimCoreService;
        }

        public ObservableCollection<PimCoreImageAsset> Assets
        {
            get => _assets;
            set => SetProperty(ref _assets, value);
        }

        protected IPimCoreService PimCoreService { get; private set; }

        public PimCoreImageAsset GetImageAssetById(int id)
        {
            return PimCoreService.ImageAssets?.Where(a => a.AssetId == id).FirstOrDefault();
        }

        protected async Task LoadImageAsset()
        {
            try
            {
                IsBusy = true;
                var lang = _selfCheckoutService.CurrentLanguage.LangCode;
                await PimCoreService.GetMediaByLocationAsync(lang.ToLower());
            }
            catch { }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
