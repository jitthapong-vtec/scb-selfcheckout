using SelfCheckout.Models;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Session
{
    public class SessionService : ISessionService
    {
        IRequestProvider _requestProvider;

        public SessionService(IRequestProvider provider)
        {
            _requestProvider = provider;
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
            var result = await _requestProvider.PutAsync<object, ApiResultData<bool>>(uri.ToString(), payload);
            return result;
        }

        public async Task<ApiResultData<SessionData>> GetDeviceStatusAsync(string machineNo)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/DeviceStatus?machine_no={machineNo}");
            var result = await _requestProvider.GetAsync<ApiResultData<SessionData>>(uri.ToString());
            return result;
        }

        public async Task<ApiResultData<SessionData>> GetSessionDetialAsync(int key)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/SessionDetail?key={key}");
            var result = await _requestProvider.GetAsync<ApiResultData<SessionData>>(uri.ToString());
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
            var result = await _requestProvider.PostAsync<object, ApiResultData<List<SessionData>>>(uri.ToString(), payload);
            return result;
        }

        public async Task<ApiResultData<int>> StartSessionAsync(string userId, string machineNo, string shoppingCartNo)
        {
            var payload = new
            {
                user_id = userId,
                machine_no = machineNo,
                shoppingcard_no = shoppingCartNo
            };

            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/Start");
            var result = await _requestProvider.PostAsync<object, ApiResultData<int>>(uri.ToString(), payload);
            return result;
        }

        public async Task<ApiResultData<bool>> UpdateSessionAsync(int sessionKey, int orderNo, string shoppingCartNo)
        {
            var payload = new
            {
                session_key = sessionKey,
                order_no = orderNo,
                shoppintcard_no = shoppingCartNo
            };

            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/Update");
            var result = await _requestProvider.PutAsync<object, ApiResultData<bool>>(uri.ToString(), payload);
            return result;
        }

        public async Task<ApiResultData<bool>> ValidateMachineAsync(string machineIp)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/ValidateMachine?machine_ip={machineIp}");
            var result = await _requestProvider.GetAsync<ApiResultData<bool>>(uri.ToString());
            return result;
        }

        public async Task<ApiResultData<bool>> ValidateShoppingCartAsync(string machineIp, string shoppingCart)
        {
            var uri = new UriBuilder($"{GlobalSettings.Instance.SelfCheckoutApi}api/Session/ValidateShoppingCard?machine_ip={machineIp}&shopping_card={shoppingCart}");
            var result = await _requestProvider.GetAsync<ApiResultData<bool>>(uri.ToString());
            return result;
        }
    }
}
