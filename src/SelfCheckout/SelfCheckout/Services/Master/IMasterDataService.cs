using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Master
{
    public interface IMasterDataService
    {
        AppConfig AppConfig { get; }

        IList<Payment> Payments { get; }

        IList<Language> Languages { get; }

        Task LoadConfigAsync();

        Task LoadLanguageAsync();

        Task LoadPaymentAsync();
    }
}
