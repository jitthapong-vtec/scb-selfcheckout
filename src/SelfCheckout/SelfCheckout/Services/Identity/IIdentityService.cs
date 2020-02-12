using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Identity
{
    public interface IIdentityService
    {
        LoginData LoginData { get; }

        Task<ApiResultData<LoginData>> LoginAsync(object user);

        Task LogoutAsync();
    }
}
