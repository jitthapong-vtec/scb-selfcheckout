using SelfCheckout.UWP.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace SelfCheckout.UWP.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            Control.GotFocus += Entry_GotFocus;
        }

        private void Entry_GotFocus(object sender, RoutedEventArgs e)
        {
            InputPane.GetForCurrentView().TryShow();
        }
    }
}
