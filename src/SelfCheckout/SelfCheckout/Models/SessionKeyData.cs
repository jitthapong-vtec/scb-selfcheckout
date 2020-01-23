using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class SessionKeyData
    {
        [JsonProperty("session_key")]
        public string SessionKey { get; set; }

        [JsonProperty("userInfo")]
        public UserInfo UserInfo { get; set; }

        [JsonProperty("dateTimeServer")]
        public DateTime DateTimeServer { get; set; }
    }

}
