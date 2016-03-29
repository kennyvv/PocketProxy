using MiNET.Items;
using PocketProxy.PC.Utils;
using PocketProxy.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class SetSlot : Packet
    {
        public SetSlot()
        {
            PacketId = 0x16;
        }

        public byte Window = 0;
        public short SlotId = 5;
        public Item Slot;

        public override void Write(MinecraftStream stream)
        {
            var mapping = ItemMapping.Pe2Pc(Slot.Id, Slot.Metadata);

            stream.WriteByte(Window);
            stream.WriteShort(SlotId);
            stream.WriteShort(mapping.Itemid);
            if (mapping.Itemid != -1 && mapping.Itemid != 0)
            {
                stream.WriteByte(Slot.Count);
                stream.WriteShort(mapping.Metadata);
                stream.WriteByte(0);
            }
        }
    }
}
