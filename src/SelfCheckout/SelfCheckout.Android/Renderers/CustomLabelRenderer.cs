using Android.Content;
using SelfCheckout.Controls;
using SelfCheckout.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace SelfCheckout.Droid.Renderers
{
    class CustomLabelRenderer : LabelRenderer
    {
        public CustomLabelRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                SetTextAllCaps();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Text")
            {
                SetTextAllCaps();
            }
        }

        void SetTextAllCaps()
        {
            var upperCase = (Element as CustomLabel).UpperCase;
            Control.SetAllCaps(upperCase);
        }
    }
}