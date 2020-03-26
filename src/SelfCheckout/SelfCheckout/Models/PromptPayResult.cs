using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class PromptPayResult
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("payeeProxyId")]
        public string PayeeProxyId { get; set; }

        [JsonProperty("payeeProxyType")]
        public string PayeeProxyType { get; set; }

        [JsonProperty("payeeAccountNumber")]
        public string PayeeAccountNumber { get; set; }

        [JsonProperty("payeeName")]
        public string PayeeName { get; set; }

        [JsonProperty("payerProxyId")]
        public string PayerProxyId { get; set; }

        [JsonProperty("payerProxyType")]
        public string PayerProxyType { get; set; }

        [JsonProperty("payerAccountNumber")]
        public string PayerAccountNumber { get; set; }

        [JsonProperty("payerName")]
        public string PayerName { get; set; }

        [JsonProperty("sendingBankCode")]
        public string SendingBankCode { get; set; }

        [JsonProperty("receivingBankCode")]
        public string ReceivingBankCode { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("channelCode")]
        public string ChannelCode { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("transactionDateandTime")]
        public string TransactionDateandTime { get; set; }

        [JsonProperty("billPaymentRef1")]
        public string BillPaymentRef1 { get; set; }

        [JsonProperty("billPaymentRef2")]
        public string BillPaymentRef2 { get; set; }

        [JsonProperty("billPaymentRef3")]
        public string BillPaymentRef3 { get; set; }

        [JsonProperty("currencyCode")]
        public int CurrencyCode { get; set; }

        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }
    }
}
