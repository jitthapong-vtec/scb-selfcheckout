using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfCheckout.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionBarView : Grid
    {
        public static readonly BindableProperty BackButtonVisibleProperty
            = BindableProperty.Create("BackButtonVisible", typeof(bool), typeof(ActionBarView), false, propertyChanged: BackButtonVisiblePropertyChanged);

        public static readonly BindableProperty HeaderLogoVisibleProperty
            = BindableProperty.Create("HeaderLogoVisible", typeof(bool), typeof(ActionBarView), true, propertyChanged: HeaderLogoVisiblePropertyChanged);

        private static void BackButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as ActionBarView).backButton.IsVisible = (bool)newValue;
        }

        private static void HeaderLogoVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as ActionBarView).headerLogo.IsVisible = (bool)newValue;
        }

        public bool BackButtonVisible
        {
            get => (bool)GetValue(BackButtonVisibleProperty);
            set => SetValue(BackButtonVisibleProperty, value);
        }

        public bool HeaderLogoVisible
        {
            get => (bool)GetValue(HeaderLogoVisibleProperty);
            set => SetValue(HeaderLogoVisibleProperty, value);
        }

        public ActionBarView()
        {
            InitializeComponent();
        }
    }
}