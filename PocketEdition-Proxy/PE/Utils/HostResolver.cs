using System.Net;

namespace PocketProxy.PE.Utils
{
    public static class HostResolver
    {
        public static IPAddress ResolveAddress(string address)
        {
            IPAddress outAddress;
            if (IPAddress.TryParse(address, out outAddress))
            {
                return outAddress;
            }
            return Dns.GetHostEntry(address).AddressList[0];
        }
    }
}
