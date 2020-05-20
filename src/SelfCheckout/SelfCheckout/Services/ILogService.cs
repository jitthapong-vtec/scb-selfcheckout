using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SelfCheckout.Services
{
    public interface ILogService
    {
        void LogDebug(string message);

        void LogInfo(string message);

        void LogError(string message, Exception ex = null);
    }
}
