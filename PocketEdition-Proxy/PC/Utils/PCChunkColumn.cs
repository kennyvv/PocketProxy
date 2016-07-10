using System;
using System.Collections.Generic;
using System.IO;
using MiNET.Entities;
using MiNET.Worlds;

namespace PocketProxy.PC.Utils
{
    public class PcChunkColumn
    {
        public static int Width = 16;
        public static int Depth = 16;
        public static int Height = 256;

        public byte[] Biome { get; set; }
        public Chunk[] Chunks { get; set; }
        public int X { get; set; }
        public int Z { get; set; }

        public List<Entity> Entities { get; set; }

        public PcChunkColumn(int x, int z)
        {
            Entities = new List<Entity>();
            Biome = ArrayOf<byte>.Create(256, 1);
            Chunks = new Chunk[16];
            for (int i = 0; i < 16; i++)
            {
                Chunks[i] = new Chunk() { Y = i, X = x, Z = z };
            }

            X = x;
            Z = z;
        }

        public ushort GetBlockId(int x, int y, int z)
        {
            var y2 = y >> 4;
            y2 = Math.Abs(y2);
            if (y2 >= 16 || y2 < 0) return 0;

            Chunk c = Chunks[y2];
            
            return c.GetBlock(x, y - (y2 * 16), z);
        }

        public byte GetMetadata(int x, int y, int z)
        {
            var y2 = y >> 4;
            y2 = Math.Abs(y2);
            if (y2 >= 16 || y2 < 0) return 0;

            Chunk c = Chunks[y2];

            return c.GetMetadata(x, y - (y2 * 16), z);
        }

        public void SetBlock(int x, int y, int z, ushort blockid, byte metadata)
        {
            var y2 = y >> 4;
            var c = Chunks[y2];
            c.SetBlock(x, y - (y2 * 16), z, blockid, metadata);
            Chunks[y2] = c;
        }

        public ChunkColumnData GetChunkData(bool skyLight)
        {
            byte[] data;

            int mask = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                using (MinecraftStream stream = new MinecraftStream(ms))
                {
                    for (int index = 0; index < Chunks.Length; index++)
                    {
                        Chunk chunk = Chunks[index];
                        if (chunk == null) continue;

                        mask |= 1 << index;
                        chunk.GetBlocks().WriteTo(stream);
                        stream.WriteByteArray(chunk.Blocklight.Data);
                        if (skyLight)
                        {
                            stream.WriteByteArray(chunk.Skylight.Data);
                        }
                    }
                    stream.Flush();
                }
                data = ms.ToArray();
            }
            return new ChunkColumnData(data, mask);
        }

        private Tuple<ushort, byte> GetCorrectBlockId(byte blockid, byte metadata)
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
			else if (blockid == 198) //Grass Path
			{
				blockid = 208;
			}
            return new Tuple<ushort, byte>(blockid, metadata);
        }

        private void ConvertPe2Pc(ChunkColumn sourceChunk)
        {
            for (var x = 0; x < 16; x++)
                for (var z = 0; z < 16; z++)
                    for (var y = 0; y < 128; y++)
                    {
                        var peIndex = (x*2048) + (z*128) + y;

                        var original = sourceChunk.blocks[peIndex];
                        var originalMeta = sourceChunk.metadata[peIndex];

                        var block = GetCorrectBlockId(original, originalMeta);
                        SetBlock(x, y, z, block.Item1, block.Item2);
                    }
        }

        public static PcChunkColumn GetPcChunkColumn(ChunkColumn sourceChunk)
        {
            PcChunkColumn c = new PcChunkColumn(sourceChunk.x, sourceChunk.z);
            c.ConvertPe2Pc(sourceChunk);
            return c;
        }

        public struct ChunkColumnData
        {
            public byte[] Data { get; }
            public int Bitmask { get; }

            public ChunkColumnData(byte[] data, int bitmask)
            {
                Data = data;
                Bitmask = bitmask;
            }
        }
    }
}