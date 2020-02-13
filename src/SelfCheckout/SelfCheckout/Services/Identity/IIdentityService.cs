using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Identity
{
    public interface IIdentityService
    {
        LoginData LoginData { get; set; }

        Task<ApiResultData<LoginData>> LoginAsync(string branchNo, string moduleCode, string machineIp, string userCode, string userPassword);

        Task LogoutAsync();
    }
}
