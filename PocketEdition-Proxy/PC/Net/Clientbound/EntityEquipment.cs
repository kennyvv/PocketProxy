using PocketProxy.PC.Utils;

namespace PocketProxy.PC.Net.Clientbound
{
    public class EntityEquipment : Packet
    {
        public int EntityId { get;set; }
        public EquipmentSlot Slot { get; set; }
        public short ItemId { get; set; }
        public short Metadata { get; set; }

        public EntityEquipment()
        {
            PacketId = 0x3C;
        }

        public override void Write(MinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteVarInt((int)Slot);
            stream.WriteShort(ItemId);
            if (ItemId != -1)
            {
                stream.WriteByte(1);
                stream.WriteShort(Metadata);
                stream.WriteByte(0);
            }
        }
    }

    public enum EquipmentSlot
    {
        MainHand = 0,
        OffHand = 1,
        Boots = 2,
        Leggings = 3,
        Chestplate = 4,
        Helmet = 5
    }
}
