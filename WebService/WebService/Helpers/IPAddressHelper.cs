using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace WebService.Helpers
{
    public static class IPAddressHelper
    {
        /// <summary>
        /// Gets all the IP addresses of the server machine hosting the application.
        /// </summary>
        /// <returns>a string array containing all the IP addresses of the server machine</returns>
        public static IPAddress[] GetIPAddresses()
            => Dns.GetHostEntry(Dns.GetHostName()).AddressList;

        /// <summary>
        /// Gets the IP address of the server machine hosting the application.
        /// </summary>
        /// <param name="num">if set, it will return the Nth available IP address: if not set, the first available one will be returned.</param>
        /// <returns>the (first available or chosen) IP address of the server machine</returns>
        public static IPAddress GetIPAddress(int num = 0)
            => GetIPAddresses()[num];

        /// <summary>
        /// Checks if the given IP address is one of the IP addresses registered to the server machine hosting the application.
        /// </summary>
        /// <param name="ipAddress">the IP Address to check</param>
        /// <returns>TRUE if the IP address is registered, FALSE otherwise</returns>
        public static bool HasIPAddress(IPAddress ipAddress)
            => GetIPAddresses().Contains(ipAddress);

        /// <summary>
        /// Checks if the given IP address is one of the IP addresses registered to the server machine hosting the application.
        /// </summary>
        /// <param name="ipAddress">the IP Address to check</param>
        /// <returns>TRUE if the IP address is registered, FALSE otherwise</returns>
        public static bool HasIPAddress(string ipAddress)
            => HasIPAddress(IPAddress.Parse(ipAddress));

        /// <summary>
        /// Returns the IPv4 address of the specified host name or IP address.
        /// </summary>
        /// <param name="sHostNameOrAddress">The host name or IP address to resolve.</param>
        /// <returns>The first IPv4 address associated with the specified host name, or null.</returns>
        public static string GetIPv4Address(string sHostNameOrAddress)
        {
            // Get the list of IP addresses for the specified host
            var aIPHostAddresses = Dns.GetHostAddresses(sHostNameOrAddress);

            // First try to find a real IPV4 address in the list
            foreach (var ipHost in aIPHostAddresses)
                if (ipHost.AddressFamily == AddressFamily.InterNetwork)
                    return ipHost.ToString();

            // If that didn't work, try to lookup the IPV4 addresses for IPV6 addresses in the list
            foreach (var ipHost in aIPHostAddresses)
            {
                if (ipHost.AddressFamily != AddressFamily.InterNetworkV6)
                    continue;

                var ihe = Dns.GetHostEntry(ipHost);
                foreach (var ipEntry in ihe.AddressList)
                    if (ipEntry.AddressFamily == AddressFamily.InterNetwork)
                        return ipEntry.ToString();
            }

            return null;
        }
    }
}