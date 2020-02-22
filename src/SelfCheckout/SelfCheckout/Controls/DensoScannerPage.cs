using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.Controls
{
    public class DensoScannerPage : ContentPage
    {
        public static readonly BindableProperty ScanCommandProperty
            = BindableProperty.Create("ScanCommand", typeof(object), typeof(DensoScannerPage), BindingMode.TwoWay);

        public ICommand ScanCommand
        {
            get => (ICommand)GetValue(ScanCommandProperty);
            set
            {
                SetValue(ScanCommandProperty, value);
            }
        }

        public event EventHandler ScanButtonClick;

        public void FireScanEvent()
        {
            ScanButtonClick?.Invoke(this, null);
        }
    }
}
