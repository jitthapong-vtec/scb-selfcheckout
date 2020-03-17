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

        int BorrowSessionKey { get; }

        string StartedShoppingCard { get; set; }

        string CurrentShoppingCard { get; set; }

        Task LoadConfigAsync();

        Task LoadLanguageAsync();

        Task LoadPaymentAsync();

        Task<int> StartSessionAsync(string userId, string machineNo, string shoppingCardNo);

        Task<bool> UpdateSessionAsync(int sessionKey, int orderNo, string shoppingCardNo);

        Task<bool> EndSessionAsync(int sessionKey, string userId, string machineNo);

        Task<SessionData> GetSessionDetialAsync(string key);

        Task<SessionData> GetDeviceStatusAsync(string machineNo);

        Task<List<SessionData>> GetSessionHistory(DateTime? date, int sessionKey, string machineNo);

        Task<bool> ValidateMachineAsync(string machineIp);

        Task<bool> ValidateShoppingCardAsync(string machineIp, string shoppingCard);
    }
}
