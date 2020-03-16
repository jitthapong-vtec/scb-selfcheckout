using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.SelfCheckout
{
    public interface ISelfCheckoutService
    {
        AppConfig AppConfig { get; }

        IList<Payment> Payments { get; }

        IList<Language> Languages { get; }

        Language CurrentLanguage { get; set; }

        string CurrentSessionKey { get; }

        string StartedShoppingCard { get; set; }

        string CurrentShoppingCard { get; set; }

        Task LoadConfigAsync();

        Task LoadLanguageAsync();

        Task LoadPaymentAsync();

        Task<ApiResultData<int>> StartSessionAsync(string userId, string machineNo, string shoppingCardNo);

        Task<ApiResultData<bool>> UpdateSessionAsync(string sessionKey, int orderNo, string shoppingCardNo);

        Task<ApiResultData<bool>> EndSessionAsync(string sessionKey, string userId, string machineNo);

        Task<ApiResultData<SessionData>> GetSessionDetialAsync(string key);

        Task<ApiResultData<SessionData>> GetDeviceStatusAsync(string machineNo);

        Task<ApiResultData<List<SessionData>>> GetSessionHistory(DateTime? date, int sessionKey, string machineNo);

        Task<ApiResultData<bool>> ValidateMachineAsync(string machineIp);

        Task<ApiResultData<bool>> ValidateShoppingCardAsync(string machineIp, string shoppingCard);
    }
}
