using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels.Base
{
    public interface ITabbedView
    {
        Task TabChangeAsync();
    }
}
