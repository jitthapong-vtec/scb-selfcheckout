using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SelfCheckout.Models
{
    public class SimpleSelectedItem : INotifyPropertyChanged
    {
        string _text1;
        string _text2;
        object _arg1;
        bool _selected;

        public string Text1
        {
            get => _text1;
            set
            {
                _text1 = value;
                NotifyPropertyChange();
            }
        }

        public string Text2
        {
            get => _text2;
            set
            {
                _text2 = value;
                NotifyPropertyChange();
            }
        }

        public object Arg1
        {
            get => _arg1;
            set
            {
                _arg1 = value;
                NotifyPropertyChange();
            }
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                NotifyPropertyChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
