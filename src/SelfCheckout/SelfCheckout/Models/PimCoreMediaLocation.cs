using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class PimCoreMediaLocation
    {
        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("listMedia")]
        public List<ListMedia> ListMedia { get; set; }
    }

    public class ListMedia
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }
}
