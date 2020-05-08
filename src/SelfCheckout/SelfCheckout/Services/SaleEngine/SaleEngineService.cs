﻿using Newtonsoft.Json;
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

            SetRequestHeader("Authorization", $"Bearer {GlobalSettings.KPApiKey}");
            SetRequestHeader("CallerID", GlobalSettings.CallerId);
        }

        public LoginData LoginData { get; set; }

        public string CouponCode { get => OrderData?.TotalBillingAmount?.CurrentValueAdjust?.VaDetail?.Code; }

        public IList<Currency> Currencies { get; private set; }

        public Currency CurrencySelected { get; set; }

        public OrderData OrderData { get; set; }

        public async Task<List<OrderData>> ActionItemToOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/ActionItemToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result.Data;
        }

        public async Task<List<OrderData>> ActionListItemToOrderAsync(object payload, bool updateOrderData = true)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/ActionListItemToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            if (updateOrderData)
                OrderData = result.Data.FirstOrDefault();
            return result.Data;
        }

        public async Task<List<OrderData>> ActionOrderPaymentAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/ActionOrderPayment");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result.Data;
        }

        public async Task<OrderPayment> ActionPaymentToOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/ActionPaymentToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result.Data?.FirstOrDefault().OrderPayments?.FirstOrDefault();
        }

        public async Task<List<OrderData>> AddItemToOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/AddItemToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result.Data;
        }

        public async Task<List<OrderData>> AddPaymentToOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/AddPaymentToOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result.Data;
        }

        public async Task<List<OrderData>> CheckoutPaymentOrder(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/CheckOutPaymentOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result.Data;
        }

        public async Task<List<OrderData>> FinishPaymentOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/FinishPaymentOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result.Data;
        }

        public async Task<List<OrderData>> GetOrderAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetOrder");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessageCode, result.DefaultMessage);
            OrderData = result.Data.FirstOrDefault();
            return result.Data;
        }

        public async Task<List<OrderData>> GetOrderListAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetOrderList");
            var result = await PostAsync<object, ApiResultData<List<OrderData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result.Data;
        }

        public async Task<Wallet> GetWalletTypeFromBarcodeAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetWalletTypeFromBarcode");
            var result = await PostAsync<object, ApiResultData<Wallet>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result.Data;
        }

        public async Task LoadCurrencyAsync(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/GetCurrency");
            var result = await PostAsync<object, ApiResultData<List<Currency>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            Currencies = result.Data;
        }

        public async Task<LoginData> LoginAsync(string username, string password)
        {
            var payload = new
            {
                branch_no = _selfCheckoutService.AppConfig.BranchNo,
                module_code = _selfCheckoutService.AppConfig.Module,
                user_code = username,
                user_password = password,
                machine_ip = GlobalSettings.Instance.MachineIp
            };
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/Authen/LoginAuthen");
            var result = await PostAsync<object, ApiResultData<LoginData>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result.Data;
        }

        public async Task LogoutAsync()
        {
            var payload = new
            {
                SessionKey = LoginData.SessionKey
            };
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/SignOut");
            var result = await PostAsync<object, ApiResultData<bool>>(uri.ToString(), payload);
        }

        public async Task<List<FullTaxInvoice>> PrintTaxInvoice(object payload)
        {
            var uri = new UriBuilder($"{_selfCheckoutService.AppConfig.UrlSaleEngineApi}api/SaleEngine/PrintTaxInvoice");
            var result = await PostAsync<object, ApiResultData<List<FullTaxInvoice>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result.Data;
        }

        public async Task VoidPaymentAsync(string merchantId, string partnerTransId)
        {
            try
            {
                var payload = new object[]
                {
                new
                {
                    MerchantId = merchantId,
                    PartnerTransId = partnerTransId
                }
                };

                var uri = new UriBuilder("http://kpservices.kingpower.com/KPPaymentGatewayAPI/api/PaymentGateway/Cancel");
                await PostAsync<object, object>(uri.ToString(), payload);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
