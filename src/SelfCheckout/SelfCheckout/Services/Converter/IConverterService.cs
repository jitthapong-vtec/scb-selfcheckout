using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Converter
{
    public interface IConverterService
    {
        Task<TResult> Convert<TResult>(string data);
    }
}
