using SelfCheckout.Models;
using SelfCheckout.Services.Master;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        IMasterDataService _appConfigService;
        IRequestProvider _requestProvider;

        public IdentityService(IMasterDataService appConfigService, IRequestProvider request)
        {
            _appConfigService = appConfigService;
            _requestProvider = request;
        }

        public LoginData LoginData { get; set; }

        public async Task<ApiResultData<LoginData>> LoginAsync(string branchNo, string moduleCode, string matchineIp, string userCode, string userPassword)
        {
            var payload = new
            {
                branch_no = branchNo,
                module_code = moduleCode,
                user_code = userCode,
                user_password = userPassword,
                machine_ip = matchineIp
            };
            var uri = new UriBuilder($"{_appConfigService.AppConfig.UrlSaleEngineApi}api/Authen/LoginAuthen");
            var response = await _requestProvider.PostAsync<object, ApiResultData<LoginData>>(uri.ToString(), payload, GlobalSettings.AccessKey);
            return response;
        }

        public Task LogoutAsync()
        {
            LoginData = null;
            return Task.FromResult(true);
        }
    }
}
