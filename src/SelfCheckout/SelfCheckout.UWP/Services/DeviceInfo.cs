using SelfCheckout.Services.Device;
using SelfCheckout.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.System.Profile;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceInfo))]
namespace SelfCheckout.UWP.Services
{
    public class DeviceInfo : IDeviceInformation
    {
        public string GetDeviceCode()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
            {
                var token = HardwareIdentification.GetPackageSpecificToken(null);
                var hardwareId = token.Id;
                var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

                byte[] bytes = new byte[16];
                dataReader.ReadBytes(bytes);

                return BitConverter.ToString(bytes).Replace("-", "");
            }
            throw new Exception("Can't get device Id");
        }

        public string GetDeviceIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames().Where(hn => 
                            hn.Type == Windows.Networking.HostNameType.Ipv4 &&
                            hn.IPInformation?.NetworkAdapter != null && 
                            hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId).FirstOrDefault();
            return hostname?.CanonicalName;
        }
    }
}
