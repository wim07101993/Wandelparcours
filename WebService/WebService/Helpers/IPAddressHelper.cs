using System.Linq;
using System.Net;

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
    }
}