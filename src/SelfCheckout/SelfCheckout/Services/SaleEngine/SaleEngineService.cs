using Newtonsoft.Json;
using SelfCheckout.Exceptions;
using SelfCheckout.Models;
using SelfCheckout.Services.Base;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.Services.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.SaleEngine
{
    public class SaleEngineService : HttpClientBase, ISaleEngineService
    {
        ISelfCheckoutService _selfCheckoutService;

        public SaleEngineService(ISelfCheckoutService selfCheckoutService, ISerializeService serializeService) : base(serializeService)
        {
            _selfCheckoutService = selfCheckoutService;

            SetRequestHeader("Authorization", $"Bearer {GlobalSettings.AccessKey}");
            SetRequestHeader("CallerID", "SCBCHECKOUT");
        }

        public LoginData LoginData { get; set; }

        public IList<Currency> Currencies { get; private set; }

        public OrderData OrderData { get; private set; }

        public async Task<ApiResultData<List<OrderData>>> ActionItemToOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/ActionItemToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result;
        }

        public async Task<ApiResultData<List<OrderData>>> ActionListItemToOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/ActionListItemToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result;
        }

        public async Task<ApiResultData<List<OrderData>>> AddItemToOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/AddItemToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result;
        }

        public async Task<ApiResultData<List<OrderData>>> AddPaymentToOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/AddPaymentToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if(!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result;
        }

        public async Task<ApiResultData<List<OrderData>>> CheckoutPaymentOrder(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/CheckOutPaymentOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result;
        }

        public async Task<ApiResultData<List<OrderData>>> FinishPaymentOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/FinishPaymentOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result;
        }

        public async Task<ApiResultData<List<OrderData>>> GetOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result;
        }

        public async Task<ApiResultData<List<OrderData>>> GetOrderListAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetOrderList");
            return await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
        }

        public async Task<ApiResultData<Wallet>> GetWalletTypeFromBarcodeAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetWalletTypeFromBarcode");
            return await PostAsync<object, ApiResultData<Wallet>>(uri.ToString(), payload);
        }

        public async Task LoadCurrencyAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetCurrency");
            var result = await PostAsync<object, ApiResultData<List<Currency>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            Currencies = result.Data;
        }

        public async Task<ApiResultData<LoginData>> LoginAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/Authen/LoginAuthen");
            var result = await PostAsync<object, ApiResultData<LoginData>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result;
        }

        public Task LogoutAsync()
        {
            LoginData = null;
            return Task.FromResult(true);
        }
    }
}
