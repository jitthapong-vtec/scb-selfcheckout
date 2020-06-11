using Android.Content;
using NLog;
using NLog.Config;
using SelfCheckout.Anddroid.Services;
using SelfCheckout.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly:Dependency(typeof(LogService))]
namespace SelfCheckout.Anddroid.Services
{
    public class LogService : ILogService
    {
        private Logger _logger;

        public LogService()
        {
            Context context = Android.App.Application.Context;
            var logPath = context.GetExternalFilesDir("");

            var config = new NLog.Config.LoggingConfiguration();
            var fileName = Path.Combine(logPath.Path, $"{DateTime.Today.ToString("yyyyMMdd")}.log");
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = fileName,
                Layout = "${longdate} ${level} ${message}  ${exception} ${event-properties:myProperty}"
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            LogManager.Configuration = config;

            _logger = LogManager.GetCurrentClassLogger();
        }

        public void LogDebug(string message)
        {
            if (!IsEnableLog())
                return;
            _logger.Debug(message);
        }

        public void LogError(string message, Exception ex = null)
        {
            if (!IsEnableLog())
                return;
            _logger.Error(ex, message);
        }

        public void LogInfo(string message)
        {
            if (!IsEnableLog())
                return;
            _logger.Info(message);
        }

        bool IsEnableLog()
        {
            return Preferences.Get("enable_log", true);
        }
    }
}
