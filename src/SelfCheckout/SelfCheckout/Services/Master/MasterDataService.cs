using SelfCheckout.Exceptions;
using SelfCheckout.Models;
using SelfCheckout.Services.RequestProvider;
using SelfCheckout.Services.Serializer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Master
{
    public class MasterDataService : IMasterDataService
    {
        IRequestProvider _requestProvider;

        public MasterDataService(IRequestProvider request)
        {
            _requestProvider = request;
        }

        public AppConfig AppConfig { get; private set; } = new AppConfig()
        {
            UrlSaleEngineApi = "https://kpservices.kingpower.com/portal/developer/SaleEngineAPI/",
            UrlRegisterApi = "https://kpservices.kingpower.com/portal/developer/registerapi/"
        };

        public IList<Payment> Payments { get; private set; }

        public IList<Language> Languages { get; private set; }

        public async Task LoadConfigAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Master/GetConfig");
            var result = await _requestProvider.GetAsync<ApiResultData<AppConfig>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            AppConfig = result.Data;
        }

        public async Task LoadLanguageAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Master/LanguageList");
            var result = await _requestProvider.GetAsync<ApiResultData<IList<Language>>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            Languages = result.Data;
        }

        public async Task LoadPaymentAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Master/PaymentList");
            var result = await _requestProvider.GetAsync<ApiResultData<IList<Payment>>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            Payments = result.Data;
        }
    }
}
