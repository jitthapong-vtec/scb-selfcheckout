using SelfCheckout.Exceptions;
using SelfCheckout.Models;
using SelfCheckout.Services.Base;
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
        public PaymentService(ISerializeService converterService) : base(converterService)
        {
            SetRequestHeaderWithoutValidation("Authorization", "7kohkIZXZbIXULjr+Qycjdxh9katA7EYgNcnUXku9Mo=");
        }

        public async Task<string> GeneratePPQrCode(object payload)
        {
            var uri = new UriBuilder("https://kpw.vtec-system.com:4455/api/qrcreate");
            var result = await PostAsync<object, PromptPayQRCode>(uri.ToString(), payload);
            if (result.Status.Code != 1000)
            {
                throw new NotiApiException(result.Status.Description);
            }
            return result.Data.QrRawData;
        }

        public async Task<PromptPayResult> InquiryAsync(string refField)
        {
            var uri = new UriBuilder($"https://kpw.vtec-system.com:4455/api/noti/billPaymentRef1/{refField}");
            var result = await GetAsync<List<PromptPayResult>>(uri.ToString());
            return result.FirstOrDefault();
        }
    }
}
