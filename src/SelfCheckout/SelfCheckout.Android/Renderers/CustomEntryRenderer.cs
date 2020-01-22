using System.ComponentModel;
using Android.Content;
using Android.Widget;
using SelfCheckout.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace SelfCheckout.Droid.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                var editText = (EditText)Control;
                editText.SetSelectAllOnFocus(true);
                editText.SetBackgroundResource(Resource.Drawable.selector_edittext_background);
            }
        }
    }
}
