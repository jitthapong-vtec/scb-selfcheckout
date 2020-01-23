using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class UserInfo
    {
        [JsonProperty("branch_no")]
        public string BranchNo { get; set; }

        [JsonProperty("user_code")]
        public string UserCode { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("user_pwd")]
        public string UserPwd { get; set; }

        [JsonProperty("moduleCode")]
        public string ModuleCode { get; set; }

        [JsonProperty("MachineEnv")]
        public Env MachineEnv { get; set; }

        [JsonProperty("list_authorize")]
        public Authorize[] ListAuthorize { get; set; }
    }

}
