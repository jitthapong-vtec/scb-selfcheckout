using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Identity
{
    public interface IIdentityService
    {
        SessionKeyData SessionData { get; }

        Task<ApiResultData<SessionKeyData>> LoginAsync(UserInput user);

        Task LogoutAsync();
    }
}
