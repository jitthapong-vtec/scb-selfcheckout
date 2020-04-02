using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace SelfCheckout.Models
{
    public class DeviceStatus : SessionData, INotifyPropertyChanged
    {
        public DeviceStatus()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    NotifyPropertyChanged("TimeUse");
                    return SessionStatus?.SessionCode == "START" ? true : false;
                });
            });
        }

        public string StartOccupied
        {
            get
            {
                if (SessionStatus.SessionCode == "START")
                    return SessionDt.ToString("HH:mm");
                else
                    return "None";
            }
        }

        public string TimeUse
        {
            get
            {
                var timeUse = new TimeSpan(0, 0, 0);
                if (SessionStatus.SessionCode == "START")
                {
                    timeUse = DateTime.Now.Subtract(SessionDt);
                }
                return timeUse.ToString("hh\\:mm\\:ss");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
