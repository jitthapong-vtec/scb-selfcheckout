using SelfCheckout.Models;
using SelfCheckout.Services.Converter;
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
        IMasterDataService _appConfigService;
        IRequestProvider _requestProvider;

        public IdentityService(IMasterDataService appConfigService, IRequestProvider request)
        {
            _appConfigService = appConfigService;
            _requestProvider = request;
        }

        public LoginData LoginData { get; private set; }

        public async Task<ApiResultData<LoginData>> LoginAsync(object user)
        {
            var uri = new UriBuilder($"{_appConfigService.AppConfig.UrlSaleEngineApi}api/Authen/LoginAuthen");
            var response = await _requestProvider.PostAsync<object, ApiResultData<LoginData>>(uri.ToString(), user, GlobalSettings.AccessKey);
            LoginData = response.Data;
            return response;
        }

        public Task LogoutAsync()
        {
            LoginData = null;
            return Task.FromResult(true);
        }
    }
}
