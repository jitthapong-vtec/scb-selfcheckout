using Android.Content;
using Android.Net.Wifi;
using Android.Text.Format;
using Android.Util;
using SelfCheckout.Anddroid.Services;
using SelfCheckout.Services.Device;
using Xamarin.Forms;
using static Android.Provider.Settings;

[assembly: Dependency(typeof(DeviceInformation))]
namespace SelfCheckout.Anddroid.Services
{
    public class DeviceInformation : IDeviceInformation
    {
        public string GetDeviceCode()
        {
            var context = Android.App.Application.Context;
            var resolver = context.ContentResolver;
            return Secure.GetString(resolver, Secure.AndroidId);
        }

        public string GetDeviceIp()
        {
            var context = Android.App.Application.Context;
            WifiManager wm = (WifiManager)context.GetSystemService(Context.WifiService);
            string ip = Formatter.FormatIpAddress(wm.ConnectionInfo.IpAddress);
            return ip;
        }
    }
}
