
using fNbt;
using MiNET.Utils;
using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class UpdateBlockEntity : Packet
    {
        public UpdateBlockEntity()
        {
            PacketId = 0x09;
        }

        public Vector3 Location { get; set; }
        public byte Action { get; set; }
        public NbtCompound Data { get; set; }
        public override void Write(MinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteByte(Action);
            stream.WriteNBTCompound(Data);
        }
    }
}
