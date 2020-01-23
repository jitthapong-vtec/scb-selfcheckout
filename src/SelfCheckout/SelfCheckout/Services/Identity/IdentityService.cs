using SelfCheckout.Models;
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
        IRequestProvider _requestProvider;
        IApiResponseConverter _apiResponseConverter;

        public IdentityService(IRequestProvider request, IApiResponseConverter converter)
        {
            _requestProvider = request;
            _apiResponseConverter = converter;
        }

        public LoginData LoginData { get; private set; }

        public async Task LoginAsync(UserInput user)
        {
            var url = new UriBuilder($"{GlobalSettings.SaleEngineBaseUrl}/api/Authen/LoginAuthen");
            var response = await _requestProvider.PostAsync(url.ToString(), user, GlobalSettings.AccessKey);
            LoginData = await _apiResponseConverter.Convert<LoginData>(response);
        }

        public Task LogoutAsync()
        {
            LoginData = null;
            return Task.FromResult(true);
        }
    }
}
