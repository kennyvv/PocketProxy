using MiNET.Utils;
using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class Explosion : Packet
    {
        public float X;
        public float Y;
        public float Z;
        public float Radius;
        public Records Records;
        public float PlayerMotionX;
        public float PlayerMotionY;
        public float PlayerMotionZ;

        public Explosion()
        {
            PacketId = 0x1C;
        }

        public override void Write(MinecraftStream stream)
        {
            stream.WriteFloat(X);
            stream.WriteFloat(Y);
            stream.WriteFloat(Z);
            stream.WriteFloat(Radius);
            stream.WriteInt(Records.Count);
            foreach (var record in Records)
            {
                stream.WriteUInt8((byte) record.X);
                stream.WriteUInt8((byte) record.Y);
                stream.WriteUInt8((byte) record.Z);
            }
            stream.WriteFloat(PlayerMotionX);
            stream.WriteFloat(PlayerMotionY);
            stream.WriteFloat(PlayerMotionZ);
        }

        public override void Read(MinecraftStream stream)
        {

        }
    }
}
