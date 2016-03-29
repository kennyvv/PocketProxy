using MiNET.Utils;
using MiNET.Worlds;

namespace PocketProxy.PC.Utils
{
    public class Chunk
    {
        private BlockStorage BlockStorage { get; set; }
        public NibbleArray Skylight = new NibbleArray(16 * 16 * 16);
        public NibbleArray Blocklight = new NibbleArray(16 * 16 * 16);
        public int[] BiomeColor = ArrayOf<int>.Create(256, 1);
        public bool Overworld = true;

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Chunk()
        {
            BlockStorage = new BlockStorage();

            for (var i = 0; i < Skylight.Length; i++)
                Skylight[i] = 255;

            for (var i = 0; i < Blocklight.Length; i++)
                Blocklight[i] = 0;

            X = 0;
            Z = 0;
            Y = 0;
        }

        internal BlockStorage GetBlocks()
        {
            return BlockStorage;
        }

        public ushort GetBlock(int x, int y, int z)
        {
            var d = BlockStorage.Get(x, y, z);
            var id = d >> 4;
            var meta = d & 0x1F;

            return (ushort) id;
        }

        public byte GetMetadata(int x, int y, int z)
        {
            var d = BlockStorage.Get(x, y, z);
            var id = d >> 4;
            var meta = d & 0x1F;

            return (byte) meta;
        }

        public void SetBlock(int x, int y, int z, ushort id, byte metadata)
        {
            BlockStorage.Set(x, y, z, id << 4 | metadata);
        }

        public void SetBlocklight(int x, int y, int z, byte data)
        {
            Blocklight[(x * 256) + (z * 16) + y] = data;
        }

        public byte GetBlocklight(int x, int y, int z)
        {
            return Blocklight[(x * 256) + (z * 16) + y];
        }

        public byte GetSkylight(int x, int y, int z)
        {
            return Skylight[(x * 256) + (z * 16) + y];
        }

        public void SetSkylight(int x, int y, int z, byte data)
        {
            Skylight[(x * 256) + (z * 16) + y] = data;
        }
    }
}
