using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class Respawn : Packet
    {
        public Respawn()
        {
            PacketId = 0x33;
        }

        public int Dimension = 0;
        public byte Difficulty = 1;
        public byte Gamemode = 0;
        public string LevelType = "default";

        public override void Write(MinecraftStream stream)
        {
            stream.WriteInt(Dimension);
            stream.WriteByte(Difficulty);
            stream.WriteByte(Gamemode);
            stream.WriteString(LevelType);
        }
    }
}
