using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.RequestProvider
{
    public interface IRequestProvider
    {
        Task<TResult> GetAsync<TResult>(string uri, string accessToken = "");

        Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string accessToken = "");
    }
}
