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

        public async Task<PimCoreApiResult<PimCoreImageAsset>> GetImageByAssetIdAsync(int id, string ratio)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PimCoreUrl}restful/getImageByAssetId/{id}?apikey={GlobalSettings.PimCoreApiKey}&ratio={ratio}");
            return await GetAsync<PimCoreApiResult<PimCoreImageAsset>>(uri.ToString());
        }

        public async Task<PimCoreApiResult<PimCoreMediaLocation>> GetMediaByLocationAsync(string lang)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PimCoreUrl}restful/getMediaByLocation?apikey={GlobalSettings.PimCoreApiKey}&location=self-checkout&lang={lang}&app=SeftCheckout");
            return await GetAsync<PimCoreApiResult<PimCoreMediaLocation>>(uri.ToString());
        }

        public async Task<PimCoreApiResult<List<PimCorePromoImage>>> GetPromoImageAsync(string key, string lang = "en")
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PimCoreUrl}restful/getPromoKios?apikey={GlobalSettings.PimCoreApiKey}&key={key}&lang={lang}");
            return await GetAsync<PimCoreApiResult<List<PimCorePromoImage>>>(uri.ToString());
        }
    }
}
