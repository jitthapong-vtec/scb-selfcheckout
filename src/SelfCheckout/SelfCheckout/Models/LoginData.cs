using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SelfCheckout.Models
{
    public class LoginData
    {
        [JsonProperty("session_key")]
        public string SessionKey { get; set; }

        [JsonProperty("userInfo")]
        public UserInfo UserInfo { get; set; }

        [JsonProperty("dateTimeServer")]
        public DateTime DateTimeServer { get; set; }
    }

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
        public MachineEnv MachineEnv { get; set; }

        [JsonProperty("list_authorize")]
        public List<ListAuthorize> ListAuthorize { get; set; }
    }

    public class ListAuthorize
    {
        [JsonProperty("ModuleCode")]
        public string ModuleCode { get; set; }

        [JsonProperty("AuthCode")]
        public string AuthCode { get; set; }

        [JsonProperty("Action")]
        public string Action { get; set; }
    }

    public class MachineEnv
    {
        [JsonProperty("MachineIP")]
        public string MachineIp { get; set; }

        [JsonProperty("MachineNo")]
        public string MachineNo { get; set; }

        [JsonProperty("MachineName")]
        public string MachineName { get; set; }

        [JsonProperty("MachineTax")]
        public string MachineTax { get; set; }

        [JsonProperty("CashierIP")]
        public string CashierIp { get; set; }

        [JsonProperty("CashierNo")]
        public string CashierNo { get; set; }

        [JsonProperty("CashierName")]
        public string CashierName { get; set; }

        [JsonProperty("CashierTax")]
        public string CashierTax { get; set; }

        [JsonProperty("ComName")]
        public string ComName { get; set; }

        [JsonProperty("IPAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("VatCode")]
        public string VatCode { get; set; }

        [JsonProperty("VatRate")]
        public long VatRate { get; set; }

        [JsonProperty("CurrCode")]
        public string CurrCode { get; set; }

        [JsonProperty("CurrRate")]
        public long CurrRate { get; set; }

        [JsonProperty("CurrShort")]
        public string CurrShort { get; set; }

        [JsonProperty("PriceList")]
        public string PriceList { get; set; }

        [JsonProperty("DChannel")]
        public string DChannel { get; set; }

        [JsonProperty("CondTypeCode")]
        public string CondTypeCode { get; set; }

        [JsonProperty("ConectionString")]
        public string ConectionString { get; set; }

        [JsonProperty("OrderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("ShopingCard")]
        public string ShopingCard { get; set; }

        [JsonProperty("OrderNo")]
        public long OrderNo { get; set; }

        [JsonProperty("ConsolFlag")]
        public bool ConsolFlag { get; set; }

        [JsonProperty("AutoTake")]
        public bool AutoTake { get; set; }

        [JsonProperty("BranchAutoTake")]
        public bool BranchAutoTake { get; set; }

        [JsonProperty("ModuleVersion")]
        public string ModuleVersion { get; set; }

        [JsonProperty("Plant")]
        public string Plant { get; set; }

        [JsonProperty("posType")]
        public long PosType { get; set; }

        [JsonProperty("currentOrdetDate")]
        public DateTime CurrentOrdetDate { get; set; }

        [JsonProperty("platFormCode")]
        public string PlatFormCode { get; set; }

        [JsonProperty("ECommerceFlag")]
        public bool ECommerceFlag { get; set; }

        [JsonProperty("UrgentTBagHour")]
        public long UrgentTBagHour { get; set; }

        [JsonProperty("UrgentTBagMinute")]
        public long UrgentTBagMinute { get; set; }

        [JsonProperty("HoldTbagDay")]
        public long HoldTbagDay { get; set; }

        [JsonProperty("CashierMac")]
        public string CashierMac { get; set; }

        [JsonProperty("buylimitHour")]
        public long BuylimitHour { get; set; }

        [JsonProperty("signPad")]
        public bool SignPad { get; set; }
    }
}
