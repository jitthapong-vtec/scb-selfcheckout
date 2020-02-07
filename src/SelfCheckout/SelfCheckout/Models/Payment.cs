using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class Payment
    {
        [JsonProperty("method_code")]
        public string MethodCode { get; set; }
        [JsonProperty("method_desc")]
        public string MethodDesc { get; set; }
        [JsonProperty("method_short")]
        public string MethodShort { get; set; }
        [JsonProperty("method_hot")]
        public string MethodHot { get; set; }
        [JsonProperty("images")]
        public string Images { get; set; }
        [JsonProperty("remark")]
        public string Remark { get; set; }
        [JsonProperty("method_group")]
        public string MethodGroup { get; set; }
        [JsonProperty("check_voucher")]
        public bool CheckVoucher { get; set; }
        [JsonProperty("is_voucher")]
        public bool IsVoucher { get; set; }
        [JsonProperty("barcode_fix")]
        public bool BarcodeFix { get; set; }
        [JsonProperty("is_cashcard")]
        public bool IsCashcard { get; set; }
        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }
        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }
        [JsonProperty("is_commission")]
        public bool IsCommission { get; set; }
        [JsonProperty("isAlipay")]
        public bool IsAlipay { get; set; }
    }
}
