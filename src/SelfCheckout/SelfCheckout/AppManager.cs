using SelfCheckout.Resources;
using SelfCheckout.Services.Localize;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout
{
    public class AppManager
    {
        static AppManager _instance;
        static object syncRoot = new object();

        public static AppManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new AppManager();
                    }
                }
                return _instance;
            }
        }

        public void InitLanguage()
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                CultureInfo ci = new CultureInfo(CountryCode);
                AppResources.Culture = ci;
                DependencyService.Get<ILocalize>().SetLocale(ci);
            }
        }

        public string CountryCode
        {
            get
            {
                var code = Preferences.Get("lang_code", "");
                if (string.IsNullOrEmpty(code))
                {
                    CultureInfo ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                    code = ci.Name;
                }
                return code;
            }
            set => Preferences.Set("lang_code", value);
        }
    }
}
