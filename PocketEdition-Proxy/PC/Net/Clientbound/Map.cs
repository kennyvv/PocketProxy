using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class Map : Packet
    {
        public Map()
        {
            PacketId = 0x24;
        }

        public int MapId { get; set; }
        public byte Scale { get; set; }
        public bool TrackingPositions { get; set; }
        public MapIcon[] MapIcons { get; set; }
        public byte Columns { get; set; }
        public byte Rows { get; set; }
        public byte X { get; set; }
        public byte Z { get; set; }
      //  public int Length { get; set; }
        public byte[] Colors { get; set; }

        public override void Write(MinecraftStream stream)
        {
            stream.WriteVarInt(MapId);
            stream.WriteUInt8(Scale);
            stream.WriteBoolean(TrackingPositions);
            stream.WriteVarInt(MapIcons.Length);
            foreach (var i in MapIcons)
            {
                stream.WriteUInt8((byte) (i.Direction | i.Type)); //TODO: Correct this
                stream.WriteUInt8(i.X);
                stream.WriteUInt8(i.Z);
            }
            stream.WriteUInt8(Columns);
            if (Columns == 0) return;
            stream.WriteUInt8(Rows);
            stream.WriteUInt8(X);
            stream.WriteUInt8(Z);
            stream.WriteVarInt(Colors.Length);
            stream.WriteBytes(Colors);
        }
    }
}
