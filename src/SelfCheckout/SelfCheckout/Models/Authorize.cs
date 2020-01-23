using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class Authorize
    {
        [JsonProperty("ModuleCode")]
        public string ModuleCode { get; set; }

        [JsonProperty("AuthCode")]
        public string AuthCode { get; set; }

        [JsonProperty("Action")]
        public long Action { get; set; }
    }
}
