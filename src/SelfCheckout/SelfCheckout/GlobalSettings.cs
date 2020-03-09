using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Localize;
using System.Globalization;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout
{
    public class GlobalSettings
    {
        public const string AccessKey = "WLR7xq7jrA5t4TF7z3JLTkBFKmskmANQ";
        public const string CallerId = "SCBCHECKOUT";

        public const string PimCoreApiKey = "db4dd68cb67e35107ad2f9f78a316a42ae981466ce095498af3000ced075d00f";

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

        public string CountryCode
        {
            get
            {
                var code = Preferences.Get("lang_code", "");
                if (string.IsNullOrEmpty(code))
                {
                    if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
                    {
                        CultureInfo ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                        code = ci.Name;
                    }
                    else
                    {
                        code = "en-US";
                    }
                }
                return code;
            }
            set => Preferences.Set("lang_code", value);
        }

        public string SelfCheckoutApi {
            get => Preferences.Get("self_checkout_api", "https://kpservices.kingpower.com/portal/developer/selfcheckoutapi/");
        }

        public string PimCoreUrl { get => "https://pim.kingpower.com/"; }

        public string MachineIp { get => "127.0.0.1"; }
    }
}
