using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Authen
{
    public interface ILoginService
    {
        Task<LoginData> LoginAsync(UserInput user);
    }
}
