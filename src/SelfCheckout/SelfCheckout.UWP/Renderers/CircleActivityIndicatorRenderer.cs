using SelfCheckout.UWP.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ActivityIndicator), typeof(CircleActivityIndicatorRenderer))]
namespace SelfCheckout.UWP.Renderers
{
    class CircleActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, ProgressRing>
    {
        ProgressRing ring;
        protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    ring = new ProgressRing();
                    ring.Width = 96;
                    ring.Height = 96;
                    ring.Foreground = SolidColorBrushFromArgb("#ff0B56A4");
                    ring.IsActive = true;
                    ring.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ring.IsEnabled = true;
                    SetNativeControl(ring);
                }
            }
        }

        public SolidColorBrush SolidColorBrushFromArgb(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }
    }
}
