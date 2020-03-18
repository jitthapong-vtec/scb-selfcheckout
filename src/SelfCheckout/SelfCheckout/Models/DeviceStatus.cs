using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class DeviceStatus : SessionData
    {
        public bool IsAvailable { get => SessionStatus.DisplayStatus == "Available"; }
    }
}
