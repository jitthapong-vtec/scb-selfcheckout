using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class FullTaxInvoice
    {
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Value")]
        public object Value { get; set; }

        [JsonProperty("Qty")]
        public long Qty { get; set; }

        [JsonProperty("Data")]
        public InvoiceData Data { get; set; }
    }

    public class InvoiceData
    {
        [JsonProperty("Original")]
        public List<Copy> Original { get; set; }

        [JsonProperty("Copy")]
        public List<Copy> Copy { get; set; }

        [JsonProperty("Confirm")]
        public List<object> Confirm { get; set; }
    }

    public class Copy
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public Uri Value { get; set; }
    }
}
