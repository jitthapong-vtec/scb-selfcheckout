using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Exceptions
{
    public class NotiApiException : Exception
    {
        public NotiApiException(string message) : base(message) { }

        public NotiApiException(string message, Exception inner) : base(message, inner) { }
    }
}
