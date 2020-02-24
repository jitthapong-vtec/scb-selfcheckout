using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Services.PimCore;
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

        ObservableCollection<PimCoreImageAsset> _assets;

        public ObservableCollection<PimCoreImageAsset> Assets
        {
            get => _assets;
            set
            {
                _assets = value;
                RaisePropertyChanged(() => Assets);
            }
        }

        public HomeViewModel(IPimCoreService pimCoreService)
        {
            _pimCoreService = pimCoreService;
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
                var result = await _pimCoreService.GetMediaByLocationAsync();
                if (result.Status == "success")
                {
                    Assets = new ObservableCollection<PimCoreImageAsset>();
                    foreach (var media in result.Data.ListMedia)
                    {
                        try
                        {
                            var assetResult = await _pimCoreService.GetImageByAssetIdAsync(media.Id, "400x200");
                            if (assetResult.Status == "success")
                            {
                                var asset = assetResult.Data;
                                asset.DetailTitle = "Detail title";
                                asset.DetailDesc = "Detail desc";
                                asset.DetailLink = media.Link;
                                Assets.Add(asset);
                            }
                        }
                        catch { }
                    }
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
