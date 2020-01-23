using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class Env
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
        public double VatRate { get; set; }

        [JsonProperty("CurrCode")]
        public string CurrCode { get; set; }

        [JsonProperty("CurrRate")]
        public double CurrRate { get; set; }

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
        public int OrderNo { get; set; }

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
        public int PosType { get; set; }

        [JsonProperty("currentOrdetDate")]
        public DateTime CurrentOrdetDate { get; set; }

        [JsonProperty("platFormCode")]
        public string PlatFormCode { get; set; }

        [JsonProperty("ECommerceFlag")]
        public bool ECommerceFlag { get; set; }

        [JsonProperty("UrgentTBagHour")]
        public int UrgentTBagHour { get; set; }

        [JsonProperty("UrgentTBagMinute")]
        public int UrgentTBagMinute { get; set; }

        [JsonProperty("HoldTbagDay")]
        public int HoldTbagDay { get; set; }

        [JsonProperty("CashierMac")]
        public string CashierMac { get; set; }

        [JsonProperty("buylimitHour")]
        public int BuylimitHour { get; set; }

        [JsonProperty("signPad")]
        public bool SignPad { get; set; }
    }
}
