using System.IO;
using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net
{
    public class Packet
    {
        public Packet()
        {
            PacketId = 0x00;
        }

        public int PacketId { get; protected set; }

        public virtual void Read(MinecraftStream stream)
        {
        }

        public virtual void Write(MinecraftStream stream)
        {
        }

        public void Read(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var stream = new MinecraftStream(ms))
                {
                    Read(stream);
                }
            }
        }

        public
            byte[] GetData()
        {
            using (var ms = new MemoryStream())
            {
                using (var stream = new MinecraftStream(ms))
                {
                    Write(stream);
                    stream.Flush();
                }
                return ms.ToArray();
            }
        }
    }
}
