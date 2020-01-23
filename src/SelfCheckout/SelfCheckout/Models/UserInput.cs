using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class UserInput
    {
        [JsonProperty("branch_no")]
        public string BranchNo { get; set; }

        [JsonProperty("module_code")]
        public string ModuleCode { get; set; }

        [JsonProperty("user_code")]
        public string UserCode { get; set; }

        [JsonProperty("user_password")]
        public string UserPassword { get; set; }

        [JsonProperty("machine_ip")]
        public string MachineIp { get; set; }
    }
}
