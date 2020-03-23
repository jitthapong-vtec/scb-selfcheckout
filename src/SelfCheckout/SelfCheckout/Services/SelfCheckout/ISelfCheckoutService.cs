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

        long BorrowSessionKey { get; }

        string StartedShoppingCard { get; set; }

        string CurrentShoppingCard { get; set; }

        Task LoadConfigAsync();

        Task LoadLanguageAsync();

        Task LoadPaymentAsync();

        Task<long> StartSessionAsync(string userId, string machineNo, string shoppingCardNo);

        Task<bool> UpdateSessionAsync(long sessionKey, int orderNo, string shoppingCardNo);

        Task<bool> EndSessionAsync(long sessionKey, string userId, string machineNo);

        Task<SessionData> GetSessionDetialAsync(long key);

        Task<List<DeviceStatus>> GetDeviceStatusAsync(string machineNo);

        Task<List<DeviceStatus>> GetSessionHistory(DateTime? date, long sessionKey, string machineNo);

        Task<bool> ValidateMachineAsync(string machineIp);

        Task<bool> ValidateShoppingCardAsync(string machineIp, string shoppingCard);
    }
}
