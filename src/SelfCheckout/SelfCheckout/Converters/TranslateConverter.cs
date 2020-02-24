using SelfCheckout.Services.Localize;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Forms;

namespace SelfCheckout.Converters
{
    public class TranslateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ci = culture;
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS
                || Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
            ResourceManager temp = new ResourceManager(TranslateExtension.ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
            return temp.GetString(value.ToString(), ci);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
