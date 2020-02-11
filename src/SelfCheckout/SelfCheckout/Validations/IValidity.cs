using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Validations
{
    public interface IValidity
    {
        bool IsValid { get; set; }
    }
}
