using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class EntityMetadata : Packet
    {
        public EntityMetadata()
        {
            PacketId = 0x39;
        }

        public int EntityId;
        public byte[] Metadata;

        public override void Write(MinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteBytes(Metadata);
        }
    }
}
