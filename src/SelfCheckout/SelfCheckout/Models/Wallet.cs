using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class Wallet
    {
        [JsonProperty("wallet_type")]
        public string WalletType { get; set; }

        [JsonProperty("barcode_prefix")]
        public string BarcodePrefix { get; set; }

        [JsonProperty("walletagent_master")]
        public WalletagentMaster WalletagentMaster { get; set; }
    }

    public class WalletagentMaster
    {
        [JsonProperty("wallet_agent")]
        public string WalletAgent { get; set; }

        [JsonProperty("partnertype_id")]
        public int PartnertypeId { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        [JsonProperty("wsurl")]
        public string Wsurl { get; set; }
    }
}
