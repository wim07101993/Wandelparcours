using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace WebService.Helpers
{
    public static class IPAddressHelper
    {
        public static IPAddress[] GetIPAddresses()
            => Dns.GetHostEntry(Dns.GetHostName()).AddressList;

        public static IPAddress GetIPAddress(int num = 0)
            => GetIPAddresses()[num];

        public static bool HasIPAddress(IPAddress ipAddress)
            => GetIPAddresses().Contains(ipAddress);

        public static bool HasIPAddress(string ipAddress)
            => HasIPAddress(IPAddress.Parse(ipAddress));

        public static string GetIPv4Address(string sHostNameOrAddress)
        {
            var aIPHostAddresses = Dns.GetHostAddresses(sHostNameOrAddress);

            foreach (var ipHost in aIPHostAddresses)
                if (ipHost.AddressFamily == AddressFamily.InterNetwork)
                    return ipHost.ToString();

            return aIPHostAddresses
                .Where(ipHost => ipHost.AddressFamily == AddressFamily.InterNetworkV6)
                .SelectMany(address => Dns.GetHostEntry(address).AddressList,
                    (ipHostEntry, ipEntry) => new {ipHostEntry, ipEntry})
                .Where(t => t.ipEntry.AddressFamily == AddressFamily.InterNetwork)
                .Select(t => t.ipEntry.ToString())
                .FirstOrDefault();
        }
    }
}