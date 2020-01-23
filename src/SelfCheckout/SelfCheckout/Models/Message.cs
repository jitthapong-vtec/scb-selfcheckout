using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class Message
    {
        [JsonProperty("MessageType")]
        public string MessageType { get; set; }

        [JsonProperty("MessageCode")]
        public string MessageCode { get; set; }

        [JsonProperty("MessageDesc")]
        public string MessageDesc { get; set; }
    }
}
