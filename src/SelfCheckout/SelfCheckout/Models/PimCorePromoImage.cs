using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class PimCorePromoImage
    {
        [JsonProperty("promotion_code")]
        public string PromotionCode { get; set; }

        [JsonProperty("promotion_name")]
        public object PromotionName { get; set; }

        [JsonProperty("promotion_value")]
        public object PromotionValue { get; set; }

        [JsonProperty("short_description")]
        public object ShortDescription { get; set; }

        [JsonProperty("condition")]
        public object Condition { get; set; }

        [JsonProperty("cover_image")]
        public CoverImage CoverImage { get; set; }

        [JsonProperty("print")]
        public object Print { get; set; }
    }

    public class CoverImage
    {
        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("filename")]
        public object Filename { get; set; }
    }
}
