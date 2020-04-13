using SelfCheckout.Models;
using SelfCheckout.Services.Base;
using SelfCheckout.Services.Serializer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.PimCore
{
    public class PimCoreService : HttpClientBase, IPimCoreService
    {
        public PimCoreService(ISerializeService converterService) : base(converterService)
        {
        }

        public List<PimCoreImageAsset> ImageAssets { get; private set; }

        public async Task<PimCoreApiResult<PimCoreImageAsset>> GetImageByAssetIdAsync(int id, string ratio = "400x200")
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PimCoreUrl}restful/getImageByAssetId/{id}?apikey={GlobalSettings.PimCoreApiKey}&ratio={ratio}");
            return await GetAsync<PimCoreApiResult<PimCoreImageAsset>>(uri.ToString());
        }

        public async Task GetMediaByLocationAsync(string lang)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PimCoreUrl}restful/getMediaByLocation?apikey={GlobalSettings.PimCoreApiKey}&location=self-checkout&lang={lang}&app=SeftCheckout");
            var result = await GetAsync<PimCoreApiResult<PimCoreMediaLocation>>(uri.ToString());
            if (result.Status == "success")
            {
                ImageAssets = new List<PimCoreImageAsset>();
                foreach (var media in result.Data.ListMedia)
                {
                    try
                    {
                        var assetResult = await GetImageByAssetIdAsync(media.Id);
                        if (assetResult.Status == "success")
                        {
                            var asset = assetResult.Data;
                            asset.DetailTitle = media.Title;
                            asset.DetailDesc = "";
                            asset.DetailLink = media.Link;
                            ImageAssets.Add(asset);
                        }
                    }
                    catch { }
                }
            }
        }

        public async Task<PimCoreApiResult<List<PimCorePromoImage>>> GetPromoImageAsync(string key, string lang = "en")
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PimCoreUrl}restful/getPromoKios?apikey={GlobalSettings.PimCoreApiKey}&key={key}&lang={lang}");
            return await GetAsync<PimCoreApiResult<List<PimCorePromoImage>>>(uri.ToString());
        }
    }
}
