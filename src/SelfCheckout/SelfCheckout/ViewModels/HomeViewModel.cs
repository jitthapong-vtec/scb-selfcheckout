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
            _assets = new ObservableCollection<PimCoreImageAsset>();
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
                    var assets = new List<PimCoreImageAsset>();
                    foreach (var media in result.Data.ListMedia)
                    {
                        try
                        {
                            var assetResult = await _pimCoreService.GetImageByAssetIdAsync(media.Id, "600x800");
                            if (assetResult.Status == "success")
                            {
                                var asset = assetResult.Data;
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
