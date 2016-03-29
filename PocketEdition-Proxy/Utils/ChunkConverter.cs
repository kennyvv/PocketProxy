using System;
using System.IO;
using MiNET.Worlds;
using PocketProxy.PC.Utils;

namespace PocketProxy.Utils
{
    public static class ChunkConverter
    {
        public static PcChunkColumn.ChunkColumnData GetChunkColumnData(ChunkColumn chunk)
        {
            const int bitmask = 65535;
            const int numBlocks = 16*16*256;
            const int bitsPerEntry = 13;
            const long maxEntryValue = (1L << bitsPerEntry) - 1;

            long[] data = new long[RoundToNearest((4096 * 16) * bitsPerEntry, 64) / 64];

            for (var x = 0; x < 16; x++)
                for (var z = 0; z < 16; z++)
                    for (var y = 0; y < 128; y++)
                    {
                        var peIndex = (x*2048) + (z*128) + y;

                        var correct = GetCorrectBlockId(chunk.blocks[peIndex], chunk.metadata[peIndex]);
                        var index = y << 8 | z << 4 | x;
                        var value = correct.Item1 << 4 | correct.Item2;

                        int bitIndex = index * bitsPerEntry;
                        int startIndex = bitIndex / 64;
                        int endIndex = ((index + 1) * bitsPerEntry - 1) / 64;
                        int startBitSubIndex = bitIndex % 64;
                        data[startIndex] = data[startIndex] & ~(maxEntryValue << startBitSubIndex) |
                                           (value & maxEntryValue) << startBitSubIndex;
                        if (startIndex != endIndex)
                        {
                            int endBitSubIndex = 64 - startBitSubIndex;
                            data[endIndex] = data[endIndex] >> endBitSubIndex << endBitSubIndex |
                                             (value & maxEntryValue) >> endBitSubIndex;
                        }
                    }

            byte[] tileData;

            using (MemoryStream ms = new MemoryStream())
            {
                using (MinecraftStream stream = new MinecraftStream(ms))
                {
                    stream.WriteByte(0);
                    stream.WriteVarInt(data.Length * 8);
                    foreach (var i in data)
                    {
                        stream.WriteLong(i);
                    }

                    for (int i = 0; i < numBlocks/2; i++) //BlockLight
                    {
                        stream.WriteByte(0);
                    }

                    for (int i = 0; i < numBlocks/2; i++) //SkyLight
                    {
                        stream.WriteByte(255);
                    }
                }
                tileData = ms.ToArray();
            }

            return new PcChunkColumn.ChunkColumnData(tileData, bitmask);
        }

        private static Tuple<ushort, byte> GetCorrectBlockId(byte blockid, byte metadata)
        {
            if (blockid == 158) //Dropper to slab
            {
                blockid = 44;
                metadata = 2;
            }
            else if (blockid == 157) //Activator rails to double slab
            {
                blockid = 125;
                metadata = 2;
            }
            else if (blockid == 126) //Wooden slab
            {
                blockid = 157;
                metadata = 0;
            }
            else if (blockid == 122 || blockid == 123) //Ender Egg -> Glowstone
            {
                blockid = 89;
                metadata = 0;
            }
            else if (blockid == 85)
            {
                switch (metadata)
                {
                    case 1:
                        blockid = 188;
                        break;
                    case 2:
                        blockid = 189;
                        break;
                    case 3:
                        blockid = 190;
                        break;
                    case 4:
                        blockid = 192;
                        break;
                    case 5:
                        blockid = 191;
                        break;
                }
                metadata = 0;
            }
            else if (blockid == 31) //Tallgrass
            {
                metadata = 1;
            }
            else if (blockid == 32) //Grass
            {
                blockid = 31;
                metadata = 0;
            }
            else if (blockid == 199) //Item frame
            {
                blockid = 0;
                metadata = 0;
            }
            return new Tuple<ushort, byte>(blockid, metadata);
        }
        private static int RoundToNearest(int value, int roundTo)
        {
            if (roundTo == 0)
            {
                return 0;
            }
            if (value == 0)
            {
                return roundTo;
            }
            if (value < 0)
            {
                roundTo *= -1;
            }

            int remainder = value % roundTo;
            return remainder != 0 ? value + roundTo - remainder : value;
        }
    }
}
