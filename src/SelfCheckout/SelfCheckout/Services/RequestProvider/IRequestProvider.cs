using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.RequestProvider
{
    public interface IRequestProvider
    {
        Task<TResult> GetAsync<TResult>(string uri);

        Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data);

        Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data);

        Task<TResult> DeleteAsync<TRequest, TResult>(string uri, TRequest data);
    }
}
