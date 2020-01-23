using SelfCheckout.Models;
using SelfCheckout.Services.Converter;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.LoginAuthen
{
    public class LoginAuthen : ILoginAuthen
    {
        IRequestProvider _requestProvider;
        IApiResponseConverter _apiResponseConverter;

        public LoginAuthen(IRequestProvider request, IApiResponseConverter converter)
        {
            _requestProvider = request;
            _apiResponseConverter = converter;
        }

        public async Task<LoginData> LoginAsync(UserInput user)
        {
            var url = new UriBuilder($"{AppManager.SaleEngineBaseUrl}/api/Authen/LoginAuthen");
            var response = await _requestProvider.PostAsync(url.ToString(), user, AppManager.AccessToken);
            try
            {
                return await _apiResponseConverter.Convert<LoginData>(response);
            }
            catch (Exception ex)
            {
                // try pase error model
                throw ex;
            }
        }
    }
}
