using SelfCheckout.Exceptions;
using SelfCheckout.Models;
using SelfCheckout.Services.Base;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Payment
{
    public class PaymentService : HttpClientBase, IPaymentService
    {
        ISaleEngineService _saleEngineService;

        public PaymentService(ISerializeService converterService, ISaleEngineService saleEngineService) : base(converterService)
        {
            _saleEngineService = saleEngineService;
            SetRequestHeaderWithoutValidation("Authorization", GlobalSettings.PromptPayApiKey);
        }

        public async Task<string> GeneratePPQrCode(object payload)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PromptPayApi}api/qrcreate");
            var result = await PostAsync<object, PromptPayQRCode>(uri.ToString(), payload);
            if (result.Status.Code != 1000)
            {
                throw new NotiApiException(result.Status.Description);
            }
            return result.Data.QrImage;
        }

        public string GetPaymentRefNo()
        {
            var orderData = _saleEngineService.OrderData;
            var customerName = orderData.CustomerDetail?.CustomerName?.Replace(" ", "");
            var custLen = customerName.Length;
            if (custLen > 14)
                custLen = 14;
            customerName = customerName.Substring(0, custLen);
            var orderNo = string.Format("{0:000000}", (int)orderData.HeaderAttributes.Where(attr => attr.Code == "order_no").FirstOrDefault().ValueOfDecimal);
            return $"{customerName}{orderNo}";
        }

        public async Task<PromptPayResult> InquiryAsync(string refField)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PromptPayApi}api/noti/billPaymentRef1/{refField}");
            var result = await GetAsync<List<PromptPayResult>>(uri.ToString());
            return result.FirstOrDefault();
        }

        public async Task<PromptPayResult> ScbInquiryAsync(string ref1, string ref2, DateTime? transactionDate = null)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.PromptPayApi}api/promptpay/inquiry?ref1={ref1}&ref2={ref2}&transactionDate={transactionDate}");
            var result = await GetAsync<List<PromptPayResult>>(uri.ToString());
            return result.FirstOrDefault();
        }
    }
}
