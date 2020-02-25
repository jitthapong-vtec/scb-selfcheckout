using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Exceptions
{
    public class DensoScannerException : Exception
    {
        public DensoScannerException(string message) : base(message) { }

        public DensoScannerException(string message, Exception inner) : base(message, inner) { }
    }
}
