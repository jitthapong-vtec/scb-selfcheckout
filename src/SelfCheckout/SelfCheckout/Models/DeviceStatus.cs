using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class DeviceStatus : SessionData
    {
        public bool IsAvailable { get => SessionStatus.DisplayStatus == "Available"; }

        public string StartOccupied
        {
            get
            {
                if (!IsAvailable)
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
                if (!IsAvailable)
                    timeUse = DateTime.Now.Subtract(SessionDt);
                return timeUse.ToString();
            }
        }
    }
}
