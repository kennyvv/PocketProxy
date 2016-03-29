using System;
using MiNET.Utils;
using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class MultiBlockChange : Packet
    {
        public MultiBlockChange()
        {
            PacketId = 0x10;
        }

        public int ChunkX;
        public int ChunkZ;
        public BlockRecords Blocks;

        public override void Write(MinecraftStream stream)
        {
            stream.WriteInt(ChunkX);
            stream.WriteInt(ChunkZ);
            stream.WriteVarInt(Blocks.Count);

            foreach (var i in Blocks)
            {
                byte xDistance = (byte)(i.Coordinates.X & 15);
                byte zDistance = (byte)(i.Coordinates.Z & 15);
                byte encodedByte = (byte) ((xDistance & 15) << 4 | (zDistance & 15));

                stream.WriteUInt8(encodedByte);
                stream.WriteUInt8((byte) i.Coordinates.Y);
                stream.WriteVarInt(i.Id << 4 | (i.Metadata & 15));
            }
        }
    }
}
