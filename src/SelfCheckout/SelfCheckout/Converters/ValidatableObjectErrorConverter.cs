using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace SelfCheckout.Converters
{
    public class ValidatableObjectErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errs = value as List<string>;
            int count = errs == null ? 0 : errs.Count;
            return count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
