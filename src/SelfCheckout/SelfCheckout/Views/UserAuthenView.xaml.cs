using SelfCheckout.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfCheckout.Views.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserAuthenView : ContentView
    {
        public static readonly BindableProperty LabelUsernameProperty =
            BindableProperty.Create("LabelUsername", typeof(string), typeof(UserAuthenView), "", propertyChanged: OnLabelUserNamePropertyChanged);

        private static void OnLabelUserNamePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as UserAuthenView).lblUsername.Text = (string)newValue;
        }

        public static readonly BindableProperty LabelPasswordProperty =
            BindableProperty.Create("LabelPassword", typeof(string), typeof(UserAuthenView), "", propertyChanged: OnLabelPasswordPropertyChanged);

        private static void OnLabelPasswordPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as UserAuthenView).lblPassword.Text = (string)newValue;
        }

        public string LabelUsername
        {
            get => (string)GetValue(LabelUsernameProperty);
            set => SetValue(LabelUsernameProperty, value);
        }

        public string LabelPassword
        {
            get => (string)GetValue(LabelPasswordProperty);
            set => SetValue(LabelPasswordProperty, value);
        }

        public UserAuthenView()
        {
            InitializeComponent();

            LabelUsername = AppResources.UserName;
            LabelPassword = AppResources.Password;
        }
    }
}