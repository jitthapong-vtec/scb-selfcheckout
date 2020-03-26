using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class CustomerOrder
    {
        public string SessionKey { get; set; }
        public string CustomerShoppingCard { get; set; }
        public string CustomerName { get; set; }
        public List<SesionDetail> SessionDetails { get; set; } = new List<SesionDetail>();
    }
}
