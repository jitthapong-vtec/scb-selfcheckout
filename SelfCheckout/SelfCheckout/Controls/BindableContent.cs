using Xamarin.Forms;

namespace SelfCheckout.Controls
{
    public class BindableContent : Grid
    {
        public static BindableProperty ViewProperty =
            BindableProperty.Create("View", typeof(ContentView), typeof(BindableContent), propertyChanging: OnChildViewPropertyChanged);

        public ContentView View
        {
            get => (ContentView)GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }

        private static void OnChildViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var content = bindable as BindableContent;
            content.Children.Clear();
            content.Children.Add((View)newValue);
        }
    }
}
