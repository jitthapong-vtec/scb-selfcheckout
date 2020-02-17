using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class Currency
    {
        [JsonProperty("branch_no")]
        public string BranchNo { get; set; }

        [JsonProperty("curr_code")]
        public string CurrCode { get; set; }

        [JsonProperty("curr_desc")]
        public string CurrDesc { get; set; }

        [JsonProperty("curr_rate")]
        public decimal CurrRate { get; set; }

        [JsonProperty("curr_short")]
        public string CurrShort { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }
}
