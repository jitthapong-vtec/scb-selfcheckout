using SelfCheckout.Exceptions;
using SelfCheckout.Models;
using SelfCheckout.Services.Base;
using SelfCheckout.Services.Serializer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.SelfCheckout
{
    public class SelfCheckoutService : HttpClientBase, ISelfCheckoutService
    {
        public SelfCheckoutService(ISerializeService serializeService) : base(serializeService)
        {
        }

        public int CurrentSessionKey { get; private set; }

        public string CurrentShoppingCard { get; set; }

        public AppConfig AppConfig { get; private set; }

        public IList<Payment> Payments { get; private set; }

        public IList<Language> Languages { get; private set; }

        public Language CurrentLanguage { get; set; }

        public string StartedShoppingCard { get; set; }

        public async Task LoadConfigAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Master/GetConfig");
            var result = await GetAsync<ApiResultData<AppConfig>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            AppConfig = result.Data;
        }

        public async Task LoadLanguageAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Master/LanguageList");
            var result = await GetAsync<ApiResultData<IList<Language>>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            Languages = result.Data;
        }

        public async Task LoadPaymentAsync()
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Master/PaymentList");
            var result = await GetAsync<ApiResultData<IList<Payment>>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            Payments = result.Data;
        }

        public async Task<ApiResultData<bool>> EndSessionAsync(int sessionKey, string userId, string machineNo)
        {
            var payload = new
            {
                session_key = sessionKey,
                user_id = userId,
                machine_no = machineNo
            };
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/End");
            var result = await PutAsync<object, ApiResultData<bool>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result;
        }

        public async Task<ApiResultData<SessionData>> GetDeviceStatusAsync(string machineNo)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/DeviceStatus?machine_no={machineNo}");
            var result = await GetAsync<ApiResultData<SessionData>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result;
        }

        public async Task<ApiResultData<SessionData>> GetSessionDetialAsync(string key)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/SessionDetai?key={key}");
            var result = await GetAsync<ApiResultData<SessionData>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result;
        }

        public async Task<ApiResultData<List<SessionData>>> GetSessionHistory(DateTime? date, int sessionKey, string machineNo)
        {
            var payload = new
            {
                date = date,
                session_key = sessionKey,
                machine_no = machineNo
            };

            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/SessionHistory");
            var result = await PostAsync<object, ApiResultData<List<SessionData>>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result;
        }

        public async Task<ApiResultData<int>> StartSessionAsync(string userId, string machineNo, string shoppingCardNo)
        {
            var payload = new
            {
                user_id = userId,
                machine_no = machineNo,
                shoppingcard_no = shoppingCardNo
            };

            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/Start");
            var result = await PostAsync<object, ApiResultData<int>>(uri.ToString(), payload);
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            CurrentSessionKey = result.Data;
            StartedShoppingCard = shoppingCardNo;
            return result;
        }

        public async Task<ApiResultData<bool>> UpdateSessionAsync(int sessionKey, int orderNo, string shoppingCardNo)
        {
            var payload = new
            {
                session_key = sessionKey,
                order_no = orderNo,
                shoppingcard_no = shoppingCardNo
            };

            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/Update");
            var result = await PutAsync<object, ApiResultData<bool>>(uri.ToString(), payload);
            if (!result.IsCompleted || result.Data == false)
                throw new KPApiException(result.DefaultMessage);
            return result;
        }

        public async Task<ApiResultData<bool>> ValidateMachineAsync(string machineIp)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/ValidateMachine?machine_ip={machineIp}");
            var result = await GetAsync<ApiResultData<bool>>(uri.ToString());
            if (!result.IsCompleted || result.Data == false)
                throw new KPApiException(result.DefaultMessage);
            return result;
        }

        public async Task<ApiResultData<bool>> ValidateShoppingCardAsync(string machineIp, string shoppingCard)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/ValidateShoppingCard?machine_ip={machineIp}&shopping_card={shoppingCard}");
            var result = await GetAsync<ApiResultData<bool>>(uri.ToString());
            if (!result.IsCompleted)
                throw new KPApiException(result.DefaultMessage);
            return result;
        }
    }
}
