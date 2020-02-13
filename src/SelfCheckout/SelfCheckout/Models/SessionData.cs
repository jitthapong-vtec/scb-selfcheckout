using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class SessionData
    {
        [JsonProperty("session_key")]
        public long SessionKey { get; set; }

        [JsonProperty("machine_no")]
        public string MachineNo { get; set; }

        [JsonProperty("shopping_card_name")]
        public string ShoppingCardName { get; set; }

        [JsonProperty("shopping_card")]
        public string ShoppingCard { get; set; }

        [JsonProperty("session_dt")]
        public DateTime SessionDt { get; set; }

        [JsonProperty("session_status")]
        public SessionStatus SessionStatus { get; set; }

        [JsonProperty("sesion_detail")]
        public List<SesionDetail> SesionDetail { get; set; }
    }

    public class SesionDetail
    {
        [JsonProperty("session_detail_key")]
        public long SessionDetailKey { get; set; }

        [JsonProperty("shopping_card")]
        public string ShoppingCard { get; set; }

        [JsonProperty("order_no")]
        public long OrderNo { get; set; }

        [JsonProperty("session_detail_dt")]
        public DateTime SessionDetailDt { get; set; }
    }

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
