using SelfCheckout.Models;
using SelfCheckout.Services.Converter;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Master
{
    public class MasterDataService : IMasterDataService
    {
        IRequestProvider _requestProvider;

        public MasterDataService(IRequestProvider request, IConverterService converter)
        {
            _requestProvider = request;
        }

        public AppConfig AppConfig { get; private set; }

        public IList<Payment> Payments { get; private set; }

        public IList<Language> Languages { get; private set; }

        public async Task LoadMasterData()
        {
            await LoadConfigAsync();
            await LoadLanguageAsync();
            await LoadPaymentAsync();
        }

        async Task LoadConfigAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.SelfCheckoutBaseUrl}api/Master/GetConfig");
            var result = await _requestProvider.GetAsync<ApiResultData<AppConfig>>(uri.ToString());
            AppConfig = result.Data;
        }

        async Task LoadLanguageAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.SelfCheckoutBaseUrl}api/Master/LanguageList");
            var response = await _requestProvider.GetAsync<ApiResultData<IList<Language>>>(uri.ToString());
            Languages = response.Data;
        }

        async Task LoadPaymentAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.SelfCheckoutBaseUrl}api/Master/PaymentList");
            var response = await _requestProvider.GetAsync<ApiResultData<IList<Payment>>>(uri.ToString());
            Payments = response.Data;
        }
    }
}
