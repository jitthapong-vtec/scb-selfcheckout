using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Exceptions
{
    public class KPApiException : Exception
    {
        public KPApiException(string message) : base(message) { }

        public KPApiException(string message, Exception inner) : base(message, inner) { }
    }
}
