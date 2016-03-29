using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Serverbound
{
    public class UseEntity : Packet
    {
        public int TargetEntity;
        public UseEntityType Type;
        public float X;
        public float Y;
        public float Z;

        public UseEntity()
        {
            PacketId = 0x0a;
        }

        public override void Write(MinecraftStream stream)
        {
            stream.WriteVarInt(TargetEntity);
            stream.WriteVarInt((int)Type);
        }

        public override void Read(MinecraftStream stream)
        {
            TargetEntity = stream.ReadVarInt();
            Type = (UseEntityType)stream.ReadVarInt();
            if (Type == UseEntityType.InteractAt)
            {
                X = stream.ReadFloat();
                Y = stream.ReadFloat();
                Z = stream.ReadFloat();
            }
        }

        public enum UseEntityType
        {
            Interact = 0,
            Attack = 1,
            InteractAt = 2
        }
    }
}
