using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SelfCheckout.Models
{
    public class CustomerOrder : INotifyPropertyChanged
    {
        string _customerName;

        public string LoginSession { get; set; }
        public string CustomerShoppingCard { get; set; }
        public string CustomerName {
            get => _customerName;
            set
            {
                _customerName = value;
                NotifyPropertyChanged();
            }
        }
        public List<SesionDetail> SessionDetails { get; set; } = new List<SesionDetail>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
