using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class SessionData
    {
        [JsonProperty("session_key")]
        public string SessionKey { get; set; }
        [JsonProperty("machine_no")]
        public string MachineNo { get; set; }
        [JsonProperty("shopping_card_name")]
        public string ShoppingCartName { get; set; }
        [JsonProperty("shopping_card")]
        public string ShoppingCart { get; set; }
        [JsonProperty("session_dt")]
        public DateTime SessionDateTime { get; set; }
        [JsonProperty("session_status")]
        public SessionStatus SessionStatus { get; set; }
        [JsonProperty("session_detail")]
        public List<SessionDetail> SessionDetail { get; set; }
    }
}
