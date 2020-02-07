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

        public SessionKeyData SessionData { get; private set; }

        public async Task<ApiResultData<SessionKeyData>> LoginAsync(UserInput user)
        {
            var uri = new UriBuilder($"{_appConfigService.AppConfig.UrlSaleEngineApi}api/Authen/LoginAuthen");
            var response = await _requestProvider.PostAsync<UserInput, ApiResultData<SessionKeyData>>(uri.ToString(), user, GlobalSettings.AccessKey);
            SessionData = response.Data;
            return response;
        }

        public Task LogoutAsync()
        {
            SessionData = null;
            return Task.FromResult(true);
        }
    }
}
