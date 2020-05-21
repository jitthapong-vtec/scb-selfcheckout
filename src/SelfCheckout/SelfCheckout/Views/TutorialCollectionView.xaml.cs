using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfCheckout.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialCollectionView : ContentView
    {
        public static readonly BindableProperty ShowIndicatorProperty =
            BindableProperty.Create("ShowIndicator", typeof(bool), typeof(TutorialCollectionView), false);

        public bool ShowIndicator {
            get => (bool)GetValue(ShowIndicatorProperty);
            set => SetValue(ShowIndicatorProperty, value);
        }

        public TutorialCollectionView()
        {
            InitializeComponent();
        }
    }
}