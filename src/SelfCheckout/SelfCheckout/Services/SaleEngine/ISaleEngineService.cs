﻿using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.SaleEngine
{
    public interface ISaleEngineService
    {
        LoginData LoginData { get;}

        OrderData OrderData { get; }

        IList<Currency> Currencies { get; }

        Task<ApiResultData<LoginData>> LoginAsync(object payload);

        Task<ApiResultData<List<OrderData>>> GetOrderAsync(object payload);

        Task<ApiResultData<List<OrderData>>> GetOrderListAsync(object payload);

        Task<ApiResultData<List<OrderData>>> AddItemToOrderAsync(object payload);

        Task<ApiResultData<List<OrderData>>> ActionItemToOrderAsync(object payload);

        Task<ApiResultData<List<OrderData>>> ActionListItemToOrderAsync(object payload);

        Task LoadCurrencyAsync(object payload);

        Task LogoutAsync();
    }
}
