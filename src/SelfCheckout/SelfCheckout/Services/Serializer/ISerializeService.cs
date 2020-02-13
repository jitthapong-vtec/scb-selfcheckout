using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Serializer
{
    public interface ISerializeService
    {
        Task<TResult> Serialize<TResult>(string data);
    }
}
