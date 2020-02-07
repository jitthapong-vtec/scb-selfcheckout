using SelfCheckout.Models;
using SelfCheckout.Services.Configuration;
using SelfCheckout.Services.Converter;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        IAppConfigService _appConfigService;
        IRequestProvider _requestProvider;
        IConverter _converter;

        public IdentityService(IAppConfigService appConfigService, IRequestProvider request, IConverter converter)
        {
            _appConfigService = appConfigService;
            _requestProvider = request;
            _converter = converter;
        }

        public SessionKeyData SessionData { get; private set; }

        public async Task<ApiResultData<SessionKeyData>> LoginAsync(UserInput user)
        {
            var uri = new UriBuilder($"{_appConfigService.AppConfig.UrlSaleEngineApi}api/Authen/LoginAuthen");
            var response = await _requestProvider.PostAsync(uri.ToString(), user, GlobalSettings.AccessKey);
            var result = await _converter.Convert<ApiResultData<SessionKeyData>>(response);
            SessionData = result.Data;
            return result;
        }

        public Task LogoutAsync()
        {
            SessionData = null;
            return Task.FromResult(true);
        }
    }
}
