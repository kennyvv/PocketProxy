using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Serverbound
{
    public class EncryptionResponse : Packet
    {
        public byte[] SharedSecret { get; set; }
        public byte[] VerifyToken  { get; set; }

        public EncryptionResponse()
        {
            PacketId = 0x01;
        }

        public override void Read(MinecraftStream stream)
        {
            SharedSecret = stream.ReadByteArray(stream.ReadVarInt());
            VerifyToken = stream.ReadByteArray(stream.ReadVarInt());
        }
    }
}
