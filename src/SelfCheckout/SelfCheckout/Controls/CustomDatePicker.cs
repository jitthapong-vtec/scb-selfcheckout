using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SelfCheckout.Controls
{
    public class CustomDatePicker : DatePicker
    {
        public static readonly BindableProperty CustomDateProperty =
            BindableProperty.Create("CustomDate", typeof(DateTime?), typeof(CustomDatePicker), null, BindingMode.TwoWay);

        public static readonly BindableProperty CustomMaxDateProperty =
            BindableProperty.Create("CustomMaxDate", typeof(DateTime), typeof(CustomDatePicker), DateTime.Now);

        public DateTime? CustomDate
        {
            get => (DateTime?)GetValue(CustomDateProperty);
            set => SetValue(CustomDateProperty, value);
        }

        public DateTime CustomMaxDate
        {
            get => (DateTime)GetValue(CustomMaxDateProperty);
            set => SetValue(CustomMaxDateProperty, value);
        }
    }
}
