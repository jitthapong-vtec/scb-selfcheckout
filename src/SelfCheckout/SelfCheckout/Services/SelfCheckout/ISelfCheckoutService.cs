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

        int CurrentSessionKey { get; }

        string CurrentShoppingCart { get; set; }

        Task LoadConfigAsync();

        Task LoadLanguageAsync();

        Task LoadPaymentAsync();

        Task<ApiResultData<int>> StartSessionAsync(string userId, string machineNo, string shoppingCartNo);

        Task<ApiResultData<bool>> UpdateSessionAsync(int sessionKey, int orderNo, string shoppingCartNo);

        Task<ApiResultData<bool>> EndSessionAsync(int sessionKey, string userId, string machineNo);

        Task<ApiResultData<SessionData>> GetSessionDetialAsync(int key);

        Task<ApiResultData<SessionData>> GetDeviceStatusAsync(string machineNo);

        Task<ApiResultData<List<SessionData>>> GetSessionHistory(DateTime? date, int sessionKey, string machineNo);

        Task<ApiResultData<bool>> ValidateMachineAsync(string machineIp);

        Task<ApiResultData<bool>> ValidateShoppingCartAsync(string machineIp, string shoppingCart);
    }
}
