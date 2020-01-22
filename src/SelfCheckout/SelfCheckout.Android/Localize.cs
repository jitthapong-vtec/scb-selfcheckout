using SelfCheckout.Services.Localize;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(SelfCheckout.Anddroid.Localize))]
namespace SelfCheckout.Anddroid
{
    public class Localize : ILocalize
    {
        CultureInfo _currentCulture = CultureInfo.InstalledUICulture;

        public void SetLocale(CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            _currentCulture = ci;
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            return _currentCulture;
        }
    }
}