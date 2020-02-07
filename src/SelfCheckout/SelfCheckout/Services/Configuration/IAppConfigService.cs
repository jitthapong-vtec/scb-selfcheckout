using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Configuration
{
    public interface IAppConfigService
    {
        AppConfig AppConfig { get; }

        Task GetConfigAsync();
    }
}
