using SelfCheckout.Models;
using SelfCheckout.Services.Converter;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Configuration
{
    public class AppConfigService : IAppConfigService
    {
        IRequestProvider _requestProvider;
        IConverter _converter;

        public AppConfigService(IRequestProvider request, IConverter converter)
        {
            _requestProvider = request;
            _converter = converter;
        }

        public AppConfig AppConfig { get; private set; }

        public async Task GetConfigAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.SelfCheckoutBaseUrl}api/Master/GetConfig");
            var response = await _requestProvider.GetAsync(uri.ToString());
            var result = await _converter.Convert<ApiResultData<AppConfig>>(response);
            AppConfig = result.Data;
        }
    }
}
