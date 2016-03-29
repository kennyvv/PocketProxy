using MiNET.Net;

namespace PocketProxy.PE
{
    public interface IPacketHandler
    {
        void PacketReceived(Package packet);
    }
}
