using MiNET.Items;
using MiNET.Net;
using PocketProxy.Network;

namespace PocketProxy.Utils
{
    public class InventoryManager
    {
        private PocketClient Client { get; }
        public InventoryManager(PocketClient pocketClient)
        {
            Client = pocketClient;
            Slots = new Item[46];
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = new ItemAir();
            }
            Hotbar = new int[9];
        }

        private Item[] Slots { get; }
        public int SelectedSlot = 0;
        public int[] Hotbar { get; }

        public void SetSlot(short slot, Item item, bool sendToServer = true)
        {
            if (slot <= Slots.Length)
            {
                Slots[slot] = item;
                if (sendToServer)
                {
                    if (slot >= 36)
                    {
                        slot -= 36;
                    }

                    McpeContainerSetSlot pack = new McpeContainerSetSlot
                    {
                        item = item,
                        slot = slot,
                        windowId = 0x79
                    };
                    Client.PeClient.SendPackage(pack);
                }
            }
        }

        public void DropSlot(int slot)
        {
            SetSlot((short) slot, new EmptyItem());
        }

        public Item GetCurrentItem
        {
            get { return Slots[36 + SelectedSlot]; }
        }

    }
}
