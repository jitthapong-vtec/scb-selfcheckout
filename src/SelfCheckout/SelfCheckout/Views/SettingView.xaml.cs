﻿using SelfCheckout.ViewModels;
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
    public partial class SettingView : ContentPage
    {
        public SettingView()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            (BindingContext as SettingViewModel).SaveSettings();
            return base.OnBackButtonPressed();
        }
    }
}