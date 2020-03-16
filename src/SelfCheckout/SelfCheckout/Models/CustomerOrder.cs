using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class CustomerOrder
    {
        public string SessionKey { get; set; }
        public string OrderNo { get; set; }
        public string CustomerName { get; set; }
    }
}
