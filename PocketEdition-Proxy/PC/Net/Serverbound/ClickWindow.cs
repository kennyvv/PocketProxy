using MiNET.Items;
using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Serverbound
{
    public class ClickWindow : Packet
    {
        public ClickWindow()
        {
            PacketId = 0x07;
        }

        public byte WindowId;
        public short Slot;
        public byte Button;
        public short ActionNumber;
        public byte Mode;
        public Item ClickedItem;

        public override void Read(MinecraftStream stream)
        {
            WindowId = stream.ReadUInt8();
            Slot = stream.ReadShort();
            Button = stream.ReadUInt8();
            ActionNumber = stream.ReadShort();
            Mode = stream.ReadUInt8();
            var itemId = stream.ReadShort();
            byte count = 0;
            short metadata = 0;

            if (itemId != -1)
            {
                count = stream.ReadUInt8();
                metadata = stream.ReadShort();
            }
            ClickedItem = ItemFactory.GetItem(itemId, metadata, count);
        }
    }
}
