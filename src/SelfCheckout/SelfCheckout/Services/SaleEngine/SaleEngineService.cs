using SelfCheckout.Exceptions;
using SelfCheckout.Models;
using SelfCheckout.Services.Master;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.SaleEngine
{
    public class SaleEngineService : ISaleEngineService
    {
        IMasterDataService _masterDataService;
        IRequestProvider _requestProvider;

        public SaleEngineService(IMasterDataService masterDataService, IRequestProvider request)
        {
            _masterDataService = masterDataService;
            _requestProvider = request;
        }

        public LoginData LoginData { get; set; }

        public IList<Currency> Currencies { get; private set; }

        public async Task<ApiResultData<List<OrderData>>> GetOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_masterDataService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetOrder");
            return await _requestProvider.PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload, GlobalSettings.AccessKey);
        }

        public async Task<ApiResultData<List<OrderData>>> GetOrderListAsync(object payload)
        {
            var uri = new UriBuilder($"{_masterDataService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetOrderList");
            return await _requestProvider.PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload, GlobalSettings.AccessKey);
        }

        public async Task LoadCurrencyAsync(object payload)
        {
            var uri = new UriBuilder($"{_masterDataService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetCurrency");
            var result = await _requestProvider.PostAsync<object, ApiResultData<List<Currency>>>(uri.ToString(), payload, GlobalSettings.AccessKey);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            Currencies = result.Data;
        }

        public async Task<ApiResultData<LoginData>> LoginAsync(object payload)
        {
            var uri = new UriBuilder($"{_masterDataService.AppConfig.UrlSaleEngineApi}api/Authen/LoginAuthen");
            return await _requestProvider.PostAsync<object, ApiResultData<LoginData>>(uri.ToString(), payload, GlobalSettings.AccessKey);
        }

        public Task LogoutAsync()
        {
            LoginData = null;
            return Task.FromResult(true);
        }
    }
}
