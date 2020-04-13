using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class Language
    {
        [JsonProperty("lang_code")]
        public string LangCode { get; set; }
        [JsonProperty("lang_desc")]
        public string LangDesc { get; set; }

        public string Icon { get => $"ic_{LangCode.ToLower()}"; }
    }
}
