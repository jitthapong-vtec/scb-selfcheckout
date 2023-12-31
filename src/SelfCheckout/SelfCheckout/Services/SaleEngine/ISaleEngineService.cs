﻿using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.SaleEngine
{
    public interface ISaleEngineService
    {
        OrderData OrderData { get; set; }

        string CouponCode { get; }

        LoginData LoginData { get; set; }

        IList<Currency> Currencies { get; }

        Currency CurrencySelected { get; set; }

        Task<LoginData> LoginAsync(string username, string password);

        Task<List<OrderData>> GetOrderAsync(object payload);

        Task<List<OrderData>> GetOrderListAsync(object payload);

        Task<List<OrderData>> AddItemToOrderAsync(object payload);

        Task<List<OrderData>> ActionItemToOrderAsync(object payload);

        Task<List<OrderData>> ActionListItemToOrderAsync(object payload, bool updateOrderData = true);

        Task<Wallet> GetWalletTypeFromBarcodeAsync(object payload);

        Task<List<OrderData>> CheckoutPaymentOrder(object payload);

        Task<List<OrderData>> AddPaymentToOrderAsync(object payload);

        Task<OrderPayment> ActionPaymentToOrderAsync(object payload);

        Task<List<OrderData>> ActionOrderPaymentAsync(object payload);

        Task<List<OrderData>> FinishPaymentOrderAsync(object payload);

        Task<List<FullTaxInvoice>> PrintTaxInvoice(object payload);

        Task VoidPaymentAsync(string merchantId, string partnerTransId);

        Task LoadCurrencyAsync(object payload);

        Task LogoutAsync();
    }
}
