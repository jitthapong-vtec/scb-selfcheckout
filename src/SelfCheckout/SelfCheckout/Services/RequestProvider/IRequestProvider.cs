using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.RequestProvider
{
    public interface IRequestProvider
    {
        Task<string> GetAsync(string uri, string accessToken = "");

        Task<string> PostAsync<TRequest>(string uri, TRequest data, string accessToken = "");
    }
}
