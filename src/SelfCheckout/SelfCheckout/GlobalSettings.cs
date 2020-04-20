using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Device;
using SelfCheckout.Services.Localize;
using System.Globalization;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout
{
    public class GlobalSettings
    {
        public const string KPApiKey = "WLR7xq7jrA5t4TF7z3JLTkBFKmskmANQ";
        public const string CallerId = "SCBCHECKOUT";

        public const string PimCoreApiKey = "db4dd68cb67e35107ad2f9f78a316a42ae981466ce095498af3000ced075d00f";

        public const string PromptPayApiKey = "7kohkIZXZbIXULjr+Qycjdxh9katA7EYgNcnUXku9Mo=";

        static GlobalSettings _instance;
        static object syncRoot = new object();

        public static GlobalSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new GlobalSettings();
                    }
                }
                return _instance;
            }
        }

        public void InitLanguage()
        {
            var ci = new CultureInfo(CountryCode);
            AppResources.Culture = ci;
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<ILocalize>().SetLocale(ci);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
        }

        public string CountryCode { get; set; } = "en-US";

        public string SelfCheckoutApi
        {
            get => Preferences.Get("self_checkout_api", "https://kpservices.kingpower.com/portal/developer/selfcheckoutapi/");
        }

        public string PromptPayApi
        {
            get => Preferences.Get("promptpay_api", "https://kpw.vtec-system.com:4455/");
        }

        public string PimCoreUrl { get => "https://pim.kingpower.com/"; }

        public int PromptPayTimeout { get => Preferences.Get("promptpay_timeout", 120); }

        public string MachineIp
        {
            get
            {
                var deviceCode = /*DependencyService.Get<IDeviceInformation>().GetDeviceCode(); //*/"8865edc3d6e1105a";
                var ipAddress = /*DependencyService.Get<IDeviceInformation>().GetDeviceIp(); //*/"192.168.2.22";
                return Device.RuntimePlatform == Device.Android ? deviceCode : ipAddress;
            }
        }
    }
}
