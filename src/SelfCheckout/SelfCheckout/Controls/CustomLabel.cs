using Xamarin.Forms;

namespace SelfCheckout.Controls
{
    public class CustomLabel : Label
    {
        public static readonly BindableProperty UpperCaseProperty =
            BindableProperty.Create("UpperCase", typeof(bool), typeof(CustomLabel), false);

        public static readonly BindableProperty HtmlTextProperty =
            BindableProperty.Create("HtmlText", typeof(string), typeof(CustomLabel), null);

        public bool UpperCase
        {
            get => (bool)GetValue(UpperCaseProperty);
            set => SetValue(UpperCaseProperty, value);
        }

        public string HtmlText
        {
            get => (string)GetValue(HtmlTextProperty);
            set => SetValue(HtmlTextProperty, value);
        }
    }
}
