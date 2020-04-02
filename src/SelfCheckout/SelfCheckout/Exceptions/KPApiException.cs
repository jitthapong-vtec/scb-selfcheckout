using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Exceptions
{
    public class KPApiException : Exception
    {
        public string ErrorCode { get; set; }

        public KPApiException(string message) : base(message) { }

        public KPApiException(string errCode, string message) : base(message) {
            ErrorCode = errCode;
        }

        public KPApiException(string message, Exception inner) : base(message, inner) { }
    }
}
