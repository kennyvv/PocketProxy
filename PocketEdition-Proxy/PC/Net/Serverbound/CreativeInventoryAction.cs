using MiNET.Items;
using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Serverbound
{
    public class CreativeInventoryAction : Packet
    {
        public CreativeInventoryAction()
        {
            PacketId = 0x18;
        }

        public Item Item;
        public short Slot;
        public override void Read(MinecraftStream stream)
        {
            Slot = stream.ReadShort();
            var itemId = stream.ReadShort();
            if (itemId != -1)
            {
                byte count = stream.ReadUInt8();
                short meta = stream.ReadShort();
                Item = ItemFactory.GetItem(itemId, meta, count);
            }
            else
            {
                Item = null;
            }
        }
    }
}
