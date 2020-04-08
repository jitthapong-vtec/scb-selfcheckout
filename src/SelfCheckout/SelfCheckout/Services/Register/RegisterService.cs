using SelfCheckout.Exceptions;
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

            SetRequestHeader("Authorization", $"Bearer {GlobalSettings.KPApiKey}");
            SetRequestHeader("CallerID", "SCBCHECKOUT");
        }

        public CustomerData CustomerData { get; private set; }

        public async Task<List<CustomerData>> GetCustomerAsync(string shoppingCard)
        {
            var payload = new
            {
                shoppingCard = shoppingCard,
                SubBranch = _selfCheckoutService.AppConfig.SubBranch,
                isTour = false,
                isGenPdfPromotion = false,
                isGenImgShoppingCard = false
            };
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlRegisterApi}api/Register/GetCustomer");
            var response = await PostAsync<object, ApiResultData<List<CustomerData>>>(uri.ToString(), payload);
            if (!response.IsCompleted)
                throw new KPApiException(response.DefaultMessage);
            CustomerData = response.Data.FirstOrDefault();
            return response.Data;
        }
    }
}
