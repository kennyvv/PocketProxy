using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Threading;
using log4net;
using PocketProxy.PC.Net.Clientbound;
using PocketProxy.PC.Net.Serverbound;
using MiNET.BlockEntities;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using Newtonsoft.Json;
using PocketProxy.PC;
using PocketProxy.PC.Net;
using PocketProxy.PC.Objects;
using PocketProxy.PC.Utils;
using PocketProxy.PE;
using PocketProxy.Utils;
using Animation = PocketProxy.PC.Net.Serverbound.Animation;
using CloseWindow = PocketProxy.PC.Net.Serverbound.CloseWindow;
using InventoryManager = PocketProxy.Utils.InventoryManager;
using KeepAlive = PocketProxy.PC.Net.Clientbound.KeepAlive;

namespace PocketProxy.Network
{
    public partial class PocketClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PocketClient));

        internal MiNetClient PeClient { get; set; }
        public PacketState PacketState { get; internal set; }
        private TcpClient Client { get; }
        private IPEndPoint ServerEndPoint { get; }

        private BlockingCollection<Packet> OutboundPacketQueue { get; }
        private BlockingCollection<Packet> OutboundPacketQueueSpawned { get; }
        private BlockingCollection<SpawnPlayer> SpawnPlayerQueue { get; }

        private Thread ReadingThread { get; }
        private Thread WritingThread { get; }
        private bool ShouldRead { get; set; }
        private MinecraftStream Stream { get; }
        public DateTime LastNetworkActivity { get; set; }
        private UUID PcClientUuid { get; }
        private string Username { get; set; }
        public long EntityId { get; set; }
        public Vector3 SpawnPosition { get; set; }
        public PlayerLocation CurrentPosition { get; set; }
        private GameMode Gamemode { get; set; }
        private InventoryManager InventoryManager { get; }
        private int ChunkRadius { get; set; }

        private List<string> SendPlayers { get; }
        private Dictionary<int, SpawnedPlayer> SpawnedPlayers { get; } 

        private float _walkingSpeed = 0.1f;
        private float _flyingSpeed = 0.1f;
        private bool _canFly;
        private bool _flying;
        private bool _spawned;
        private bool _invulnerable;

        private PlayerLocation CurrentPositionPe
        {
            get
            {
                PlayerLocation pl = CurrentPosition;
                pl.Y += 1.62f;
                return pl;
            }
        }

        public PocketClient(TcpClient tcpclient, IPEndPoint serverEndPoint)
        {
            _invulnerable = false;
            ChunkRadius = 12;
            InventoryManager = new InventoryManager(this);
            Gamemode = GameMode.Survival;
            SpawnPosition = Vector3.Zero;
            CurrentPosition = new PlayerLocation(SpawnPosition);
            ShouldRead = true;
            PcClientUuid = new UUID(Guid.NewGuid());
            Client = tcpclient;
            ServerEndPoint = serverEndPoint;
            PacketState = PacketState.Handshake;

            OutboundPacketQueue = new BlockingCollection<Packet>();
            OutboundPacketQueueSpawned = new BlockingCollection<Packet>();
            SpawnPlayerQueue = new BlockingCollection<SpawnPlayer>();

            SendPlayers = new List<string>();
            SpawnedPlayers = new Dictionary<int, SpawnedPlayer>();

            Stream = new MinecraftStream(tcpclient.GetStream());
            ReadingThread = new Thread(ReadNetwork);
            WritingThread = new Thread(WriteNetwork);
            LastNetworkActivity = DateTime.Now;
            _chunkUnloader = new Thread(ChunkUnloader);
            ReadingThread.Start();
            WritingThread.Start();
        }

        public void OnTick(long tick)
        {
            if (PacketState != PacketState.Play) return;

            if (tick%20 == 0) //Send Keep-Alive every second (20 ticks)
            {
                SendKeepAlive();
            }

            if (PeClient == null || !_spawned) return;

            if (OutboundPacketQueueSpawned.Count > 0)
            {
                QueuePacket(OutboundPacketQueueSpawned.Take());
            }

            if (SpawnPlayerQueue.Count > 0)
            {
                var t = SpawnPlayerQueue.Take();
                if (SendPlayers.Contains(t.PlayerUUID)) //Check if we are ready to spawn the player
                {
                    QueuePacket(t);

                    QueuePacket(new EntityLook()
                    {
                        EntityId = t.EntityId,
                        Yaw = t.Yaw,
                        Pitch = t.Pitch,
                        OnGround = false
                    });

                    QueuePacket(new Entityheadlook
                    {
                        EntityId = t.EntityId,
                        Yaw = t.Yaw
                    });

                    SendPlayers.Remove(t.PlayerUUID);
                }
                else //We are not yet ready to spawn this player.
                {
                    SpawnPlayerQueue.Add(t);
                }
            }

            //Below we unload the chunks we don't need on the client anymore
            if (DateTime.UtcNow.Subtract(_lastUnload).TotalSeconds > 10)
            {
                if (_chunkUnloader.ThreadState != ThreadState.Running)
                {
                    _chunkUnloader = new Thread(ChunkUnloader);
                    _chunkUnloader.Start();
                }
            }
        }

        private DateTime _lastUnload = DateTime.MinValue;
        private Thread _chunkUnloader;
        private void ChunkUnloader()
        {
            if (!_spawned) return;

            double radius = ChunkRadius;
            double radiusSquared = Math.Pow(radius, 2);
            double viewArea = Math.PI * radiusSquared;


            if (_loadedChunks.Count <= viewArea) return;

            var currentChunkCoordinates = new ChunkCoordinates(CurrentPosition);
            while (_loadedChunks.Count > viewArea)
            {
                try
                {
                    foreach (var chunk in new List<Tuple<int, int>>(_loadedChunks))
                    {
                        if (chunk == null) continue;

                        if (
                            currentChunkCoordinates.DistanceTo(new ChunkCoordinates(chunk.Item1, chunk.Item2)) > ChunkRadius)
                        {
                            UnloadChunk(chunk.Item1, chunk.Item2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("Error while unloading chunks!", ex);
                }
            }
            _lastUnload = DateTime.UtcNow;
        }

        public void KillProxy()
        {
            ShouldRead = false;
            PeClient?.SendDisconnectionNotification();
            PeClient?.Disconnect();
            ReadingThread.Abort();
            WritingThread.Abort();
            Thread.CurrentThread.Abort();
        }

        private void ReadPacket()
        {
            int pData;
            var length = Stream.ReadVarInt();
            var packetId = Stream.ReadVarInt(out pData);
            var data = Stream.ReadByteArray(length - pData);

            var packet = PacketFactory.CreatePacket(packetId, data, PacketState);
            HandlePacket(packet);
        }

        private void ReadNetwork()
        {
            while (ShouldRead)
            {
                if (Client.Client.Available != 0)
                {
                    ReadPacket();
                    LastNetworkActivity = DateTime.Now;
                }
                Thread.Sleep(1);
            }
        }

        private void WriteNetwork()
        {
            while (ShouldRead)
            {
                Packet packet;
                if (OutboundPacketQueue.Count > 3)
                {
                    var count = OutboundPacketQueue.Count;
                    List<Packet> packets = new List<Packet>(count);
                    for (int i = 0; i < count -1; i++)
                    {
                        packet = OutboundPacketQueue.Take();
                        packets.Add(packet);

                        if (packet.PacketId == 0x2d && packet is AddPlayerListItem)
                        {
                            SendPlayers.Add(((AddPlayerListItem)packet).UUID);
                        }
                    }
                    SendPackets(packets.ToArray());
                    continue;
                }

                packet = OutboundPacketQueue.Take();
                SendPacket(packet);

                if (packet.PacketId == 0x2d && packet is AddPlayerListItem)
                {
                    SendPlayers.Add(((AddPlayerListItem)packet).UUID);
                }
            }
        }

        private void SendPackets(Packet[] packets)
        {
            try
            {
                using (var stream = new MinecraftStream(Client.GetStream()))
                {
                    foreach (var packet in packets)
                    {
                        var packetData = packet.GetData();
                        stream.WriteVarInt(packetData.Length + MinecraftStream.GetVarIntLength(packet.PacketId));
                        stream.WriteVarInt(packet.PacketId);
                        stream.WriteByteArray(packetData);
                    }
                    stream.Flush();
                }
            }
            catch
            {
                //
            }
        }

        private void SendPacket(Packet packet)
        {
            try
            {
				var packetData = packet.GetData();
				
                using (var stream = new MinecraftStream(Client.GetStream()))
                {
                    stream.WriteVarInt(packetData.Length + MinecraftStream.GetVarIntLength(packet.PacketId));
                    stream.WriteVarInt(packet.PacketId);
                    stream.WriteByteArray(packetData);
                    stream.Flush();
                }
            }
            catch
            {
                //
            }
        }

        public void QueuePacket(Packet packet, bool spawnedOnly = false)
        {
            if (spawnedOnly)
            {
                OutboundPacketQueueSpawned.Add(packet);
                return;
            }
            OutboundPacketQueue.Add(packet);
        }

        public bool IsConnected()
        {
            try
            {
                if (Client.Client.Poll(0, SelectMode.SelectRead))
                {
                    var buff = new byte[1];
                    if (Client.Client.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SendHandItem()
        {
            var currentItem = InventoryManager.GetCurrentItem;

            McpeMobEquipment pack = McpeMobEquipment.CreateObject();
            pack.item = currentItem;
            pack.slot = (byte)(InventoryManager.SelectedSlot + 9);
            pack.selectedSlot = (byte)InventoryManager.SelectedSlot;
            PeClient.SendPackage(pack);
        }

        public void HandlePacket(Packet packet)
        {
            if (packet == null) return;

            if (packet is Handshake)
            {
                HandleHandshake((Handshake) packet);
            }
            else if (packet is Request)
            {
                HandleRequest((Request) packet);
            }
            else if (packet is Ping)
            {
                HandlePing((Ping) packet);
            }
            else if (packet is Loginstart)
            {
                HandleLoginStart((Loginstart) packet);
            }
            else if (packet is PC.Net.Serverbound.Chatmessage)
            {
                HandleChatMessage((PC.Net.Serverbound.Chatmessage) packet);
            }
            else if (packet is PC.Net.Serverbound.PlayerPositionAndLook)
            {
                HandlePlayerPositionAndLook((PC.Net.Serverbound.PlayerPositionAndLook) packet);
            }
            else if (packet is PlayerPosition)
            {
                HandlePlayerPosition((PlayerPosition) packet);
            }
            else if (packet is Clientstatus)
            {
                HandleClientStatus((Clientstatus) packet);
            }
            else if (packet is UseEntity)
            {
                HandleUseEntity((UseEntity) packet);
            }
            else if (packet is PlayerLook)
            {
                HandlePlayerLook((PlayerLook) packet);
            }
            else if (packet is Animation)
            {
                HandleAnimation((Animation) packet);
            }
            else if (packet is EntityAction)
            {
                HandleEntityAction((EntityAction) packet);
            }
            else if (packet is HeldItemChange)
            {
                HandleHeldItemChange((HeldItemChange) packet);
            }
            else if (packet is ClickWindow)
            {
                HandleClickWindow((ClickWindow) packet);
            }
            else if (packet is CloseWindow)
            {
                HandleCloseWindow((CloseWindow)packet);
            }
            else if (packet is Playerblockplacement)
            {
                HandlePlayerBlockPlacement((Playerblockplacement) packet);
            }
            else if (packet is Playerdigging)
            {
                HandlePlayerDigging((Playerdigging) packet);
            }
            else if (packet is CreativeInventoryAction)
            {
                HandleCreativeInventoryAction((CreativeInventoryAction) packet);
            }
            else if (packet is ClientSettings)
            {
                HandleClientSettings((ClientSettings) packet);
            }
        }

        private void HandleClientSettings(ClientSettings packet)
        {
            //Unused for now
        }

        private void HandleCreativeInventoryAction(CreativeInventoryAction packet)
        {
            var item = packet.Item;
            if (item == null || item.Id == -1)
            {
                item = new EmptyItem();
            }

            var slot = packet.Slot;
            if (slot >= 36)
            {
                slot -= 36;
            }

            InventoryManager.SetSlot(slot, item);
        }

        private void HandlePlayerDigging(Playerdigging packet)
        {
            if (packet.Status == 2 || (packet.Status == 0 && Gamemode == GameMode.Creative))
            {
                var x = packet.Location >> 38;
                var y = (packet.Location >> 26) & 0xFFF;
                var z = packet.Location << 38 >> 38;

                McpeRemoveBlock pack = McpeRemoveBlock.CreateObject();
                pack.x = (int)x;
                pack.y = (byte)y;
                pack.z = (int)z;
                pack.entityId = 0;
                PeClient.SendPackage(pack);

                McpeRemoveBlock pack2 = McpeRemoveBlock.CreateObject();
                pack2.x = (int)x;
                pack2.y = (byte)y;
                pack2.z = (int)z;
                pack2.entityId = 0;
                PeClient.SendPackage(pack2);
                return;
            }

            var currentItem = InventoryManager.GetCurrentItem;
            if (currentItem.Id == -1 || currentItem.Id == 0) return;

            if (packet.Status == 3) //Drop Item Stack
            {
                McpeDropItem drop = McpeDropItem.CreateObject();
                drop.itemtype = (byte)InventoryManager.SelectedSlot;
                drop.item = currentItem;
                PeClient.SendPackage(drop);

                QueuePacket(new SetSlot()
                {
                    Window = 0,
                    SlotId = (short) (36 + InventoryManager.SelectedSlot),
                    Slot = new EmptyItem()
                });

                InventoryManager.SetSlot((short) (36 + InventoryManager.SelectedSlot), new EmptyItem());
                SendHandItem();
            }

            if (packet.Status == 4) //Drop Item
            {
                var removedItem = ItemFactory.GetItem(currentItem.Id, currentItem.Metadata, currentItem.Count);
                currentItem.Count = 1;

                McpeDropItem drop = McpeDropItem.CreateObject();
                drop.itemtype = (byte) InventoryManager.SelectedSlot;
                drop.item = currentItem;
                PeClient.SendPackage(drop);

                if (removedItem.Count == 1)
                {
                    removedItem = new EmptyItem();
                }
                else
                {
                    removedItem.Count = (byte) (removedItem.Count - 1);
                }

                QueuePacket(new SetSlot()
                {
                    Window = 0,
                    SlotId = (short)(36 + InventoryManager.SelectedSlot),
                    Slot = removedItem
                });

                InventoryManager.SetSlot((short)(36 + InventoryManager.SelectedSlot), removedItem);
                SendHandItem();
            }
        }

        private void HandlePlayerBlockPlacement(Playerblockplacement packet)
        {
            var x = packet.Location >> 38;
            var y = (packet.Location >> 26) & 0xFFF;
            var z = packet.Location << 38 >> 38;

            McpeUseItem send = McpeUseItem.CreateObject();
            send.blockcoordinates = new BlockCoordinates((int) x, (int) y, (int) z);
            send.item = InventoryManager.GetCurrentItem;
            send.face = (byte) packet.Face;
            send.facecoordinates = new Vector3(packet.CursorPositionX/16f, packet.CursorPositionY/16f,
                packet.CursorPositionZ/16f);
            send.playerposition = new Vector3(CurrentPosition.X, CurrentPosition.Y, CurrentPosition.Z);
			send.unknown = new byte[4] {0,0,0,0};
            PeClient.SendPackage(send);
        }

        private void HandleCloseWindow(CloseWindow packet)
        {
            McpeContainerClose ret = McpeContainerClose.CreateObject();
            ret.windowId = packet.WindowId;
            PeClient.SendPackage(ret);

            _openedInventory = -999;
        }

        private void HandleClickWindow(ClickWindow packet)
        {
            QueuePacket(new ConfirmTransaction()
            {
                Accepted = false,
                ActionNumber = packet.ActionNumber,
                WindowId = packet.WindowId
            });
            if (packet.WindowId != 0) return;

            //TODO:
            //Correctly implement this
            //Not sure if this is even possible
        }

        private void HandleHeldItemChange(HeldItemChange packet)
        {
            InventoryManager.SelectedSlot = packet.Slot;

            SendHandItem();
        }

        private void HandleEntityAction(EntityAction packet)
        {
            var actionid = packet.ActionId;
            switch (actionid)
            {
                case 0:
                    PeClient.SendAction(PlayerAction.StartSneak);
                    break;
                case 1:
                    PeClient.SendAction(PlayerAction.StopSneak);
                    break;
                case 3:
                    PeClient.SendAction(PlayerAction.StartSprint);
                    break;
                case 4:
                    PeClient.SendAction(PlayerAction.StopSprint);
                    break;
            }
        }

        private void HandleAnimation(Animation packet)
        {
            PeClient.SendAnimation(1);
        }

        private void HandleUseEntity(UseEntity packet)
        {
            McpeInteract interact = McpeInteract.CreateObject();
            interact.actionId = (byte) packet.Type;
            interact.targetEntityId = packet.TargetEntity;
           
            PeClient.SendPackage(interact);
        }

        private bool _respawning = false;
        private void HandleClientStatus(Clientstatus packet)
        {
            if (packet.ActionId == 0) //WE REALLY WANNA RESPAWN NOW. THANK YOU
            {
                _respawning = true;

                UnloadAllChunks();

                PeClient.RequestRespawn();

                QueuePacket(new Respawn());
            }
        }

        private void HandlePlayerPosition(PlayerPosition packet)
        {
            CurrentPosition.X = (float) packet.X;
            CurrentPosition.Y = (float) packet.Y;
            CurrentPosition.Z = (float) packet.Z;

            PeClient.SendMovePlayer(CurrentPositionPe);
        }

        private void HandlePlayerLook(PlayerLook packet)
        {
            CurrentPosition.Pitch = packet.Pitch;
            CurrentPosition.Yaw = packet.Yaw;

            PeClient.SendMovePlayer(CurrentPosition);
        }

        private void HandlePlayerPositionAndLook(PC.Net.Serverbound.PlayerPositionAndLook packet)
        {
            CurrentPosition.Pitch = packet.Pitch;
            CurrentPosition.Yaw = packet.Yaw;
            CurrentPosition.X = (float) packet.X;
            CurrentPosition.Y = (float) packet.Y;
            CurrentPosition.Z = (float) packet.Z;

            PeClient.SendMovePlayer(CurrentPositionPe);
        }

        private void HandleChatMessage(PC.Net.Serverbound.Chatmessage msg)
        {
            PeClient.SendChat(msg.ChatMessage);
        }

        private void HandleLoginStart(Loginstart packet)
        {
#if DEBUG
           // Username = "Kennyvv";
            Username = packet.Name;
#else
            Username = packet.Name;
#endif

            QueuePacket(new SetCompression
            {
                Threshold = -1
            });

            QueuePacket(new LoginSuccess
            {
                UUID = PcClientUuid.ToString(),
                Username = Username
            });

            PacketState = PacketState.Play;

            ConnectToPe();
        }

        private void HandlePing(Ping ping)
        {
            QueuePacket(new Pong() {Payload = ping.Payload});
        }

        private void HandleRequest(Request request)
        {
            int online = 0;
            int max = 0;
            string motd = "\u00A76\u00A7lPocketProxy\n\u00A7cThe target server seems to be offline!";

            var serverInfo = ServerList.QueryServer(ServerEndPoint);
            if (serverInfo != null)
            {
                motd = serverInfo.MOTD;
                online = serverInfo.OnlinePlayers;
                max = serverInfo.MaxPlayers;
            }

            QueuePacket(new Response
            {
                Status = new StatusResponse(PocketProxy.ProtocolName, ProtocolInfo.ProtocolVersion, online, max, new ChatObject(motd))
            });
        }

        private void HandleHandshake(Handshake handshake)
        {
            PacketState = handshake.NextState;
        }

        private readonly List<Tuple<int, int>> _loadedChunks = new List<Tuple<int, int>>();
        private void SendChunkColumn(ChunkColumn chunk)
        {
            var index = new Tuple<int, int>(chunk.x, chunk.z);
            if (_loadedChunks.Contains(index))
            {
				UnloadChunk(chunk.x, chunk.z);
                Thread.Sleep(50);
            }

            _loadedChunks.Add(index);

            var chunkInfo = PcChunkColumn.GetPcChunkColumn(chunk);
            var chunkData = chunkInfo.GetChunkData(true);

            QueuePacket(new ChunkData()
            {
                GroundUpContinuous = true,
                ChunkX = chunk.x,
                ChunkZ = chunk.z,
                PrimaryBitmask = chunkData.Bitmask,
                SizeOfData = chunkData.Data.Length + 256,
                Data = chunkData.Data,
                Biome = chunk.biomeId
            });

            foreach (var i in chunk.BlockEntities)
            {
                var compound = i.Value;
                var nbtTag = compound.Get("id");
                if (nbtTag != null && nbtTag.StringValue == "Sign")
                {
                    Sign s = new Sign();
                    s.SetCompound(compound);
                   /* QueuePacket(new Updatesign
                    {
						 Line1 = JsonConvert.SerializeObject(new ChatObject(s.Text1)),
						 Line2 = JsonConvert.SerializeObject(new ChatObject(s.Text2)),
						 Line3 = JsonConvert.SerializeObject(new ChatObject(s.Text3)),
						 Line4 = JsonConvert.SerializeObject(new ChatObject(s.Text4)),
						Location = i.Key
                    });*/
                }
                else if (nbtTag != null && nbtTag.StringValue == "Skull")
                {
                    UpdateBlockEntity update = new UpdateBlockEntity
                    {
                        Action = 4, //Update mob head
                        Data = compound,
                        Location = i.Key
                    };
                    QueuePacket(update);
                }
            }
        }

        private void UnloadChunk(int x, int y)
        {
            QueuePacket(new UnloadChunk()
            {
                ChunkX = x, ChunkZ = y
            });

            var index = new Tuple<int, int>(x,y);

            if (!_loadedChunks.Contains(index)) return;
            _loadedChunks.Remove(index);
        }

        private void UnloadAllChunks()
        {
            foreach (var i in _loadedChunks.ToList())
            {
                UnloadChunk(i.Item1, i.Item2);
            }
        }

        internal void SendKeepAlive()
        {
            var sendPingSecret = GetRandomInt();
            QueuePacket(new KeepAlive { KeepAliveID = sendPingSecret });
        }

        private byte GetFlags()
        {
            byte flags = 0;
            ByteHelper.SetBit(ref flags, 0x08, _invulnerable); //Invulnerable
            ByteHelper.SetBit(ref flags, 0x04, _flying); //IS Flying
            ByteHelper.SetBit(ref flags, 0x02, _canFly); //Can fly
            ByteHelper.SetBit(ref flags, 0x01, Gamemode == GameMode.Creative); //Is Creative
            return flags;
        }

        private void SendPlayerAbilities()
        {
            QueuePacket(new PlayerAbilities
            {
                Flags = GetFlags(),
                FieldOfViewModifier = 0,
                FlyingSpeed = _flyingSpeed
            });
        }

        private readonly Random _random = new Random();
        private readonly object _randomLock = new object();

        private int GetRandomInt()
        {
            lock (_randomLock)
            {
                return _random.Next();
            }
        }
    }
}