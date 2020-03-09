using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Register
{
    public interface IRegisterService
    {
        CustomerData CustomerData { get;}

        Task<List<CustomerData>> GetCustomerAsync(object payload);
    }
}
