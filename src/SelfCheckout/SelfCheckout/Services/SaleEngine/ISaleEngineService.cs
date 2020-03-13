using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.SaleEngine
{
    public interface ISaleEngineService
    {
        OrderData OrderData { get; }

        LoginData LoginData { get; set; }

        IList<Currency> Currencies { get; }

        Currency CurrencySelected { get; set; }

        Currency BaseCurrency { get; }

        Task<ApiResultData<LoginData>> LoginAsync(string username, string password);

        Task<ApiResultData<List<OrderData>>> GetOrderAsync(object payload);

        Task<ApiResultData<List<OrderData>>> GetOrderListAsync(object payload);

        Task<ApiResultData<List<OrderData>>> AddItemToOrderAsync(object payload);

        Task<ApiResultData<List<OrderData>>> ActionItemToOrderAsync(object payload);

        Task<ApiResultData<List<OrderData>>> ActionListItemToOrderAsync(object payload);

        Task<ApiResultData<Wallet>> GetWalletTypeFromBarcodeAsync(object payload);

        Task<ApiResultData<List<OrderData>>> CheckoutPaymentOrder(object payload);

        Task<ApiResultData<List<OrderData>>> AddPaymentToOrderAsync(object payload);

        Task<OrderPayment> ActionPaymentToOrderAsync(object payload);

        Task<ApiResultData<List<OrderData>>> FinishPaymentOrderAsync(object payload);

        Task VoidPaymentAsync(string merchantId, string partnerTransId);

        Task LoadCurrencyAsync(object payload);

        Task LogoutAsync();
    }
}
