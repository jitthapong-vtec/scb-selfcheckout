using SelfCheckout.Services;
using SelfCheckout.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(NativeApiService))]
namespace SelfCheckout.UWP.Services
{
    public class NativeApiService : INativeApiService
    {
        public void ExitApp()
        {
            App.Current.Exit();
        }
    }
}
