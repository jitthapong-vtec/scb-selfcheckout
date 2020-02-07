using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Converter
{
    public interface IConverter
    {
        Task<TResult> Convert<TResult>(string data);
    }
}
