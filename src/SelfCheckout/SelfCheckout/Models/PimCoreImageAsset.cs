using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class PimCoreImageAsset
    {
        [JsonProperty("asset_id")]
        public long AssetId { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("ratio")]
        public string Ratio { get; set; }

        public string DetailTitle { get; set; }
        public string DetailDesc { get; set; }
        public string DetailLink { get; set; }
    }
}
