using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class SessionStatus
    {
        [JsonProperty("session_code")]
        public string SessionCode { get; set; }
        [JsonProperty("session_name")]
        public string SessionName { get; set; }
        [JsonProperty("display_status")]
        public string DisplayStatus { get; set; }
    }
}
