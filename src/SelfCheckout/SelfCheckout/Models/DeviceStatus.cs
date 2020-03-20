using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class DeviceStatus : SessionData
    {
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
                    timeUse = DateTime.Now.Subtract(SessionDt);
                return timeUse.ToString();
            }
        }
    }
}
