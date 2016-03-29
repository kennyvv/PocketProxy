using fNbt;
using MiNET.Items;
using MiNET.Utils;
using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class WindowItems : Packet
    {
        public byte WindowId { get; set; }
        public ItemStacks Slots { get; set; }
        public WindowItems()
        {
            PacketId = 0x14;
        }

        public override void Write(MinecraftStream stream)
        {
            stream.WriteByte(WindowId);
            stream.WriteShort((short) Slots.Count);
            foreach (var entry in Slots)
            {
                stream.WriteShort(entry.Id);
                if (entry.Id != -1)
                {
                    stream.WriteByte((byte) (entry.Count > 0 ? entry.Count : 1));
                    stream.WriteShort(entry.Metadata);
                    stream.WriteByte(0);
                    //NbtCompound extraData = entry.ExtraData;
                    //if (extraData == null)
                    //{
                    //stream.WriteByte(0);
                    // }
                    // else
                    // {
                    //   stream.WriteByte(0);
                    //NbtList ench = (NbtList)extraData["ench"];
                    //NbtCompound enchComp = (NbtCompound)ench[0];
                    // var id = enchComp["id"].ShortValue;
                    // var lvl = enchComp["lvl"].ShortValue;
                    // }
                }
            }
        }
    }
}
