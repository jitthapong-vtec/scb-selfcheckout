using Xamarin.Forms;

namespace SelfCheckout.Controls
{
    public class TabContent : Grid
    {
        public static BindableProperty ContentProperty =
            BindableProperty.Create("Content", typeof(ContentView), typeof(TabContent), propertyChanging: OnContentPropertyChanged);

        public ContentView Content
        {
            get => (ContentView)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private static void OnContentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var content = bindable as TabContent;
            content.Children.Clear();
            content.Children.Add((View)newValue);
        }
    }
}
