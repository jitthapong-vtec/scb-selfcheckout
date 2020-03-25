using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Services.PimCore;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        IPimCoreService _pimCoreService;
        ISelfCheckoutService _selfCheckoutService;

        ObservableCollection<PimCoreImageAsset> _assets;

        public HomeViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, IPimCoreService pimCoreService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
            _pimCoreService = pimCoreService;
        }

        public ObservableCollection<PimCoreImageAsset> Assets
        {
            get => _assets;
            set => SetProperty(ref _assets, value);
        }

        public override async Task OnTabSelected(TabItem item)
        {
            await LoadImageAsset();
        }

        async Task LoadImageAsset()
        {
            try
            {
                IsBusy = true;
                var lang = _selfCheckoutService.CurrentLanguage.LangCode;
                var result = await _pimCoreService.GetMediaByLocationAsync(lang.ToLower());
                if (result.Status == "success")
                {
                    var assets = new List<PimCoreImageAsset>();
                    foreach (var media in result.Data.ListMedia)
                    {
                        try
                        {
                            var assetResult = await _pimCoreService.GetImageByAssetIdAsync(media.Id, "400x200");
                            if (assetResult.Status == "success")
                            {
                                var asset = assetResult.Data;
                                asset.DetailTitle = media.Title;
                                asset.DetailDesc = "";
                                asset.DetailLink = media.Link;
                                assets.Add(asset);
                            }
                        }
                        catch { }
                    }
                    Assets = assets.ToObservableCollection();
                }
            }
            catch { }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
