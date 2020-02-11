using System;
using System.Globalization;
using Xamarin.Forms;

namespace SelfCheckout.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? inverse = value as bool?;
            if(inverse != null){
                return !inverse;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
