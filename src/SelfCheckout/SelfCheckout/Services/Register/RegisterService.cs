using SelfCheckout.Models;
using SelfCheckout.Services.Base;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.Services.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Register
{
    public class RegisterService : HttpClientBase, IRegisterService
    {
        ISelfCheckoutService _selfCheckoutService;

        public RegisterService(ISerializeService serialzeService, ISelfCheckoutService selfCheckoutService) : base(serialzeService)
        {
            _selfCheckoutService = selfCheckoutService;

            SetRequestHeader("Authorization", $"Bearer {GlobalSettings.AccessKey}");
            SetRequestHeader("CallerID", "SCBCHECKOUT");
        }

        public CustomerData CustomerData { get; private set; }

        public async Task<ApiResultData<List<CustomerData>>> GetCustomerAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlRegisterApi}api/Register/GetCustomer");
            var response = await PostAsync<object, ApiResultData<List<CustomerData>>>(uri.ToString(), payload);
            CustomerData = response.Data?.FirstOrDefault();
            return response;
        }
    }
}
