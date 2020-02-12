using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class SessionDetail
    {
        [JsonProperty("session_detail_key")]
        public string SessionDetailKey { get; set; }
        [JsonProperty("shopping_card")]
        public string ShoppingCart { get; set; }
        [JsonProperty("order_no")]
        public int OrderNo { get; set; }
        [JsonProperty("session_detail_dt")]
        public DateTime SessionDetailDateTime { get; set; }
    }
}
