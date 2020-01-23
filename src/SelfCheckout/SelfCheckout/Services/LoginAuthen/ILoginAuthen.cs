using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.LoginAuthen
{
    public interface ILoginAuthen
    {
        Task<LoginData> LoginAsync(UserInput user);
    }
}
