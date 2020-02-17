using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.SaleEngine
{
    public interface ISaleEngineService
    {
        LoginData LoginData { get; set; }

        IList<Currency> Currencies { get; }

        Task<ApiResultData<LoginData>> LoginAsync(object payload);

        Task LoadCurrencyAsync(object payload);

        Task LogoutAsync();
    }
}
