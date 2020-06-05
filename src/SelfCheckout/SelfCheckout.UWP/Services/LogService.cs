using SelfCheckout.Services;
using SelfCheckout.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly:Dependency(typeof(LogService))]
namespace SelfCheckout.UWP.Services
{
    public class LogService : ILogService
    {
        public void LogDebug(string message)
        {
        }

        public void LogError(string message, Exception ex = null)
        {
        }

        public void LogInfo(string message)
        {
        }
    }
}
