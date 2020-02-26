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

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace SelfCheckout.UWP.Renderers
{
    public class CustomLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                SetTextUpperCase();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Text")
            {
                SetTextUpperCase();
            }
        }

        void SetTextUpperCase()
        {
            var upperCase = (Element as CustomLabel).UpperCase;
            if (upperCase)
                Control.Text = Control.Text.ToUpper();
        }
    }
}
