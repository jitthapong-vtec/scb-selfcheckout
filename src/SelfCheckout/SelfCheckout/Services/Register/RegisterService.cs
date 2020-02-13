using SelfCheckout.Models;
using SelfCheckout.Services.Master;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Register
{
    public class RegisterService : IRegisterService
    {
        IRequestProvider _requestProvider;
        IMasterDataService _masterDataService;

        public RegisterService(IRequestProvider requestProvider, IMasterDataService masterDataService)
        {
            _requestProvider = requestProvider;
            _masterDataService = masterDataService;
        }

        public CustomerData CustomerData { get; private set; }

        public async Task<ApiResultData<List<CustomerData>>> GetCustomerAsync(object payload)
        {
            var uri = new UriBuilder($"{_masterDataService.AppConfig.UrlRegisterApi}api/Register/GetCustomer");
            var response = await _requestProvider.PostAsync<object, ApiResultData<List<CustomerData>>>(uri.ToString(), payload, GlobalSettings.AccessKey);
            CustomerData = response.Data?.FirstOrDefault();
            return response;
        }
    }
}
