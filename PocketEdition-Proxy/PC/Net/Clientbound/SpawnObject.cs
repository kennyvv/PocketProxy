using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class SpawnObject : Packet
    {
        public int EntityId;
        public string EntityUUID;
        public byte Type;
        public double X;
        public double Y;
        public double Z;
        public byte Yaw;
        public byte Pitch;
        public short VelocityX;
        public short VelocityY;
        public short VelocityZ;
        public int Data;

        public SpawnObject()
        {
            PacketId = 0x00;
        }

        public override void Write(MinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteUUID(EntityUUID);
            stream.WriteUInt8(Type);
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteUInt8(Pitch);
            stream.WriteUInt8(Yaw);
            stream.WriteInt(Data);
            stream.WriteShort(VelocityX);
            stream.WriteShort(VelocityY);
            stream.WriteShort(VelocityZ);
        }
    }
}
