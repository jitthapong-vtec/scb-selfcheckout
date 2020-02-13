using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelfCheckout.Models
{
    public class ApiResultData<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonProperty("listOfProcessTracking")]
        public ProcessTracking ListOfProcessTracking { get; set; }

        [JsonProperty("Tracking")]
        public Tracking Tracking { get; set; }

        [JsonProperty("Message")]
        public Message[] Message { get; set; }

        public string DefaultMessage { get => Message?.FirstOrDefault()?.MessageDesc; }
    }
}
