﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfCheckout.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TappedGrid : Grid
    {
        public static readonly BindableProperty TappedCommandProperty =
            BindableProperty.Create("TappedCommand", typeof(ICommand), typeof(TappedGrid), null);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter", typeof(object), typeof(TappedGrid), null);

        public event EventHandler<object> Tapped;

        public TappedGrid()
        {
            GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = TransitionCommand
            });
        }

        private ICommand TransitionCommand
        {
            get
            {
                return new Command(async () =>
                {
                    AnchorX = 0.48;
                    AnchorY = 0.48;
                    await this.ScaleTo(0.95, 50, Easing.Linear);
                    await Task.Delay(100);
                    await this.ScaleTo(1, 50, Easing.Linear);
                    TappedCommand?.Execute(CommandParameter);
                    Tapped?.Invoke(this, CommandParameter);
                });
            }
        }

        public ICommand TappedCommand
        {
            get => (ICommand)GetValue(TappedCommandProperty);
            set => SetValue(TappedCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
    }
}