using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.PimCore
{
    public interface IPimCoreService
    {
        PimCoreImageAsset[] ImageAssets { get;}

        Task GetMediaByLocationAsync(string lang = "en");

        Task<PimCoreApiResult<PimCoreImageAsset>> GetImageByAssetIdAsync(int id, string ratio = "");

        Task<PimCoreApiResult<List<PimCorePromoImage>>> GetPromoImageAsync(string key, string lang="en");
    }
}
