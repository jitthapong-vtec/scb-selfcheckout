using SelfCheckout.Models;
using SelfCheckout.Services.Master;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        IMasterDataService _masterDataService;
        IRequestProvider _requestProvider;

        public IdentityService(IMasterDataService masterDataService, IRequestProvider request)
        {
            _masterDataService = masterDataService;
            _requestProvider = request;
        }

        public LoginData LoginData { get; set; }

        public async Task<ApiResultData<LoginData>> LoginAsync(object payload)
        {
            var uri = new UriBuilder($"{_masterDataService.AppConfig.UrlSaleEngineApi}api/Authen/LoginAuthen");
            var response = await _requestProvider.PostAsync<object, ApiResultData<LoginData>>(uri.ToString(), payload, GlobalSettings.AccessKey);
            return response;
        }

        public Task LogoutAsync()
        {
            LoginData = null;
            return Task.FromResult(true);
        }
    }
}
