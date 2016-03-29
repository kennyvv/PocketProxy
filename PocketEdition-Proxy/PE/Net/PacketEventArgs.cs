using System;
using MiNET.Net;

namespace PocketProxy.PE.Net
{
    public class PacketEventArgs : EventArgs
    {
        public Package Package { get; }
        public PacketEventArgs(Package package)
        {
            Package = package;
        }
    }
}
