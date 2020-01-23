using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class Tracking
    {
        [JsonProperty("MyException")]
        public object MyException { get; set; }

        [JsonProperty("Namespace")]
        public string Namespace { get; set; }

        [JsonProperty("ClassName")]
        public string ClassName { get; set; }

        [JsonProperty("MethodName")]
        public string MethodName { get; set; }

        [JsonProperty("AssemblyInfo")]
        public string AssemblyInfo { get; set; }
    }
}
