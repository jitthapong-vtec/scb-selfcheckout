using SelfCheckout.Controls;
using SelfCheckout.UWP.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
namespace SelfCheckout.UWP.Renderers
{
    public class CustomDatePickerRenderer : ViewRenderer<DatePicker, Windows.UI.Xaml.Controls.CalendarDatePicker>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.OldElement == null)
            {
                Windows.UI.Xaml.Controls.CalendarDatePicker datePicker = new Windows.UI.Xaml.Controls.CalendarDatePicker();
                datePicker.DateChanged += DatePicker_DateChanged;
                datePicker.MaxDate = DateTime.SpecifyKind((Element as CustomDatePicker).CustomMaxDate, DateTimeKind.Utc);
                datePicker.DateFormat = "{day.integer}‎/{month.integer}/‎{year.full}";
                SetNativeControl(datePicker);
            }
        }

        private void DatePicker_DateChanged(Windows.UI.Xaml.Controls.CalendarDatePicker sender, Windows.UI.Xaml.Controls.CalendarDatePickerDateChangedEventArgs args)
        {
            (Element as CustomDatePicker).CustomDate = args.NewDate?.DateTime;
        }
    }
}
