using System;
using System.IO;
using PocketProxy.PC.Net.Clientbound;
using MiNET;
using MiNET.BlockEntities;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using Newtonsoft.Json;
using PocketProxy.PC.Objects;
using PocketProxy.PC.Utils;
using PocketProxy.Utils;
using Explosion = PocketProxy.PC.Net.Clientbound.Explosion;

namespace PocketProxy.Network
{
    public partial class PocketClient
    {
        private int _openedInventory = -999;
        private void ConnectToPe()
        {
            PeClient = new MiNetClient(ServerEndPoint, Username);
            PeClient.OnStartGame += PocketEditionClientOnOnStartGame;
            PeClient.OnChunkData += PocketEditionClient_OnChunkData;
            PeClient.OnSetSpawnPosition += PocketEditionClient_OnSetSpawnPosition;
            PeClient.OnChatMessage += PocketEditionClient_OnChatMessage;
            PeClient.OnDisconnect += PocketEditionClient_OnDisconnect;
            PeClient.OnPlayerAdd += PocketEditionClient_OnPlayerAdd;
            PeClient.OnContainerContent += PocketEditionClient_OnContainerContent;
            PeClient.OnPlayerMovement += PocketEditionClient_OnPlayerMovement;
            PeClient.OnPlayerEquipment += PocketEditionClient_OnPlayerEquipment;
            PeClient.OnMcpeUpdateAttributes += PeClient_OnMcpeUpdateAttributes;
            PeClient.OnServerDifficulty += PocketEditionClient_OnServerDifficulty;
            PeClient.OnTimeChanged += PocketEditionClient_OnTimeChanged;
            PeClient.OnEntityAdd += PocketEditionClient_OnEntityAdd;
            PeClient.OnPlayerRemoval += PocketEditionClient_OnPlayerRemoval;
            PeClient.OnMcpePlayerList += PeClient_OnMcpePlayerList;
            PeClient.OnBlockUpdate += PocketEditionClient_OnBlockUpdate;
            PeClient.OnMcpeAnimate += PocketEditionClient_OnAnimation;
            PeClient.OnMcpeRespawn += PocketEditionClient_OnRespawn;
            PeClient.OnTileEntityData += PocketEditionClient_OnTileEntityData;
            PeClient.OnSetContainerSlot += PocketEditionClient_OnSetContainerSlot;
            PeClient.OnMcpePlayerArmor += OnMcpePlayerArmor;
            PeClient.OnEntityData += PocketEditionClient_OnEntityData;
            PeClient.OnMcpeRemoveEntity += PocketEditionClient_OnMcpeRemoveEntity;
            PeClient.OnMcpePlayerStatus += PocketEditionClient_OnMcpePlayerStatus;
            PeClient.OnMcpeAddItemEntity += PocketEditionClient_OnMcpeAddItemEntity;
            PeClient.OnMcpeTakeItemEntity += PocketEditionClient_OnMcpeTakeItemEntity;
            PeClient.OnMcpeChunkRadiusUpdate += PocketEditionClient_OnMcpeChunkRadiusUpdate;
            PeClient.OnMcpeAdventureSettings += PocketEditionClient_OnMcpeAdventureSettings;
            PeClient.OnMcpeMoveEntity += PeClient_OnMcpeMoveEntity;
            PeClient.OnMcpeSetEntityMotion += PeClient_OnMcpeSetEntityMotion;
            PeClient.OnMcpeEntityEvent += PeClient_OnMcpeEntityEvent;
            PeClient.OnMcpeContainerOpen += PeClient_OnMcpeContainerOpen;
            PeClient.OnMcpeTileEvent += PeClient_OnMcpeTileEvent;
            PeClient.OnMcpeExplode += PeClient_OnMcpeExplode;
            PeClient.Connect();
        }

        private void PeClient_OnMcpePlayerList(McpePlayerList packet)
        {
            foreach (var i in packet.records)
            {
                var displayName = i.NameTag;
                var uuid = i.ClientUuid;
                if (displayName == null) //Player List Remove Packet
                {
                    QueuePacket(new RemovePlayerListItem
                    {
                        NumberOfPlayers = 1,
                        UUID = uuid.ToString()
                    });

                    if (SendPlayers.Contains(uuid.ToString()))
                    {
                        SendPlayers.Remove(uuid.ToString());
                    }

                    PcSkin.RemoveSkinFromCache(uuid.ToString());
                    return;
                }

                PcSkin.AddSkinToCache(uuid.ToString(), i.Skin.Texture);

                var rawDisplayName = displayName;
                displayName = displayName.RemoveSpecialCharacters();
                if (displayName.Length > 16)
                {
                    displayName = displayName.Substring(0, 16);
                }

                QueuePacket(new AddPlayerListItem
                {
                    Gamemode = 0,
                    HasDisplayName = false,
                    Name = displayName,
                    NumberOfPlayers = 1,
                    Ping = 0,
                    UUID = uuid.ToString(),
                    NumberOfProperties = 0
                }, true);

                QueuePacket(new UpdateDisplayName()
                {
                    UUID = uuid.ToString(),
                    HasDisplayName = true,
                    NumberOfPlayers = 1,
                    DisplayName = JsonConvert.SerializeObject(new ChatObject(rawDisplayName)),
                    Action = 3
                }, true);
            }
        }

        private void PeClient_OnMcpeExplode(McpeExplode packet)
        {
            QueuePacket(new Explosion
            {
                X = packet.x,
                Y = packet.y,
                Z = packet.z,
                Records = packet.records,
                Radius = packet.radius,
                PlayerMotionX = 0,
                PlayerMotionY = 0,
                PlayerMotionZ = 0
            });
        }

        private void PeClient_OnMcpeTileEvent(McpeTileEvent packet)
        {
            QueuePacket(new BlockAction
            {
                Location = new Vector3(packet.x, packet.y, packet.z),
                BlockType = 54,
                Byte1 = 1,
                Byte2 = (byte) (packet.case2 == 2 ? 1 : 0)
            });
        }

        private void PeClient_OnMcpeContainerOpen(McpeContainerOpen packet)
        {
            string type = "minecraft:chest";
            string title = "Unknown";

            switch (packet.type)
            {
                case 0:
                    type = "minecraft:chest";
                    title = "Chest";
                    break;
                case 4:
                    type = "minecraft:enchanting_table";
                    title = "Enchanting table";
                    break;
                case 2:
                    type = "minecraft:furnace";
                    title = "Furnace";
                    break;
            }

            if (_openedInventory != -999)
            {
                QueuePacket(new CloseWindow {WindowId = (byte) _openedInventory});
            }

            _openedInventory = packet.windowId;
            QueuePacket(new OpenWindow
            {
                WindowId = packet.windowId,
                Slots = (byte) packet.slotCount,
                Title = JsonConvert.SerializeObject(new ChatObject(title)),
                Type = type
            });
        }

        private void PeClient_OnMcpeEntityEvent(McpeEntityEvent packet)
        {
            //TODO: Translate these
        }

        private void PeClient_OnMcpeUpdateAttributes(McpeUpdateAttributes message)
        {
            if (message.entityId != 0) return;

            if (message.attributes.ContainsKey("generic.health") && message.attributes.ContainsKey("player.hunger"))
            {
                QueuePacket(new Updatehealth
                {
                    Health = message.attributes["generic.health"].Value,
                    FoodSaturation = 0f,
                    Food = (int)message.attributes["player.hunger"].Value
                });
            }

            if (message.attributes.ContainsKey("player.level") && message.attributes.ContainsKey("player.experience"))
            {
                QueuePacket(new SetExperience
                {
                    ExperienceBar = 0.0f,
                    Level = (int)message.attributes["player.level"].Value,
                    TotalExperience = (int)message.attributes["player.experience"].Value
                });
            }
        }

        private void PeClient_OnMcpeSetEntityMotion(McpeSetEntityMotion packet)
        {
            foreach (var i in packet.entities)
            {
                short speedx = (short)Math.Round(i.Value.X * 8000);
                short speedy = (short)Math.Round(i.Value.Y * 8000);
                short speedz = (short)Math.Round(i.Value.Z * 8000);

                QueuePacket(new EntityVelocity
                {
                    EntityId = (int) i.Key,
                    VelocityX = speedx,
                    VelocityY = speedy,
                    VelocityZ = speedz
                });
            }
        }

        private void PeClient_OnMcpeMoveEntity(McpeMoveEntity packet)
        {
            if (packet.entities.Count == 0) return;
            
            foreach (var i in packet.entities)
            {
                QueuePacket(new EntityTeleport
                {
                    EntityId = (int)i.Key,
                    X = i.Value.X,
                    Y = i.Value.Y,
                    Z = i.Value.Z,
                    OnGround = true,
                    Pitch = i.Value.Pitch.ToAngle(),
                    Yaw = i.Value.Yaw.ToAngle()
                });

                QueuePacket(new EntityLook
                {
                    EntityId = (int)i.Key,
                    Yaw = i.Value.Yaw.ToAngle(),
                    Pitch = i.Value.Pitch.ToAngle(),
                    OnGround = false
                });

                QueuePacket(new Entityheadlook
                {
                    EntityId = (int)i.Key,
                    Yaw = i.Value.Yaw.ToAngle()
                });
            }
        }

        private void PocketEditionClient_OnMcpeAdventureSettings(McpeAdventureSettings packet)
        {
            bool fly = (packet.flags & 0x80) > 0;
            bool fly2 = (packet.flags & 0x20) > 0;

            _canFly = fly;
            _flying = fly2;
            SendPlayerAbilities();
        }

        private void PocketEditionClient_OnMcpeChunkRadiusUpdate(McpeChunkRadiusUpdate packet)
        {
            if (!_spawned) return;
            ChunkRadius = packet.chunkRadius;
        }

        private void PocketEditionClient_OnMcpeTakeItemEntity(McpeTakeItemEntity packet)
        {
            QueuePacket(new CollectItem
            {
                CollectedEntityId = (int)packet.entityId,
                CollectorEntityId = (int)packet.target
            });
        }

        private void PocketEditionClient_OnMcpeAddItemEntity(McpeAddItemEntity packet)
        {
           // Console.WriteLine("Velocity: {0}, {1}, {2}", packet.speedX, packet.speedY, packet.speedZ);

            short speedx = (short)Math.Round(packet.speedX * 16000);
            short speedy = (short)Math.Round(packet.speedY * 16000);
            short speedz = (short)Math.Round(packet.speedZ * 16000);

            // Console.WriteLine("Velocity: {0}, {1}, {2}", speedx, speedy, speedz);
            var mapping = ItemMapping.Pe2Pc(packet.item.Id, packet.item.Metadata);

            byte[] metaData;
            using (MemoryStream ms = new MemoryStream())
            {
                using (MinecraftStream stream = new MinecraftStream(ms))
                {
                    stream.WriteUInt8(5);
                    stream.WriteUInt8(5);
                    stream.WriteShort(mapping.Itemid);
                    stream.WriteUInt8(packet.item.Count);
                    stream.WriteShort(mapping.Metadata);
                    stream.WriteUInt8(0);

                    stream.WriteUInt8(0xff);
                }
                metaData = ms.ToArray();
            }

            QueuePacket(new SpawnObject
            {
                EntityId = (int)packet.entityId,
                EntityUUID = Guid.NewGuid().ToString(),
                X = packet.x,
                Y = packet.y,
                Z = packet.z,
                VelocityX = speedx,
                VelocityY = speedy,
                VelocityZ = speedz,
                Pitch = 0,
                Yaw = 0,
                Type = 2,
               // Data = packet.item.Id << 4 | (packet.item.Metadata & 15)
            }, true);

            EntityMetadata meta = new EntityMetadata
            {
                EntityId = (int) packet.entityId,
                Metadata = metaData
            };

            QueuePacket(meta, true);
        }

        private void PocketEditionClient_OnMcpePlayerStatus(McpePlayerStatus packet)
        {
            //This isn't used at the moment

            /*if (packet.status == 3)
            {
                //We are gonna be respawning
            }
            else if (packet.status == 0)
            {
                //Login succes
            }*/
        }

        private void PocketEditionClient_OnMcpeRemoveEntity(McpeRemoveEntity packet)
        {
            QueuePacket(new Destroyentities
            {
                Count = 1,
                EntityId = (int)packet.entityId
            });
        }

        private void PocketEditionClient_OnEntityData(McpeSetEntityData message)
        {
            if (message.entityId == 0) //DO Stuff For Me
            {
                return;
            }

            //TODO: Find a good way to check what type of entity we are talking about

            if (SpawnedPlayers.ContainsKey((int) message.entityId))
            {
                SendPlayerMetadata((int) message.entityId, message.metadata);
            }
        }

        private void OnMcpePlayerArmor(McpePlayerArmorEquipment spacket)
        {
            if (spacket.entityId == 0) return;

            var mapping = ItemMapping.Pe2Pc(spacket.boots.Id, spacket.boots.Metadata);
            QueuePacket(new EntityEquipment
            {
                EntityId = (int)spacket.entityId,
                Slot = EquipmentSlot.Boots,
                ItemId = mapping.Itemid,
                Metadata = mapping.Metadata
            });

            mapping = ItemMapping.Pe2Pc(spacket.chestplate.Id, spacket.chestplate.Metadata);
            QueuePacket(new EntityEquipment
            {
                EntityId = (int)spacket.entityId,
                Slot = EquipmentSlot.Chestplate,
                ItemId = mapping.Itemid,
                Metadata = mapping.Metadata
            });

            mapping = ItemMapping.Pe2Pc(spacket.leggings.Id, spacket.leggings.Metadata);
            QueuePacket(new EntityEquipment
            {
                EntityId = (int)spacket.entityId,
                Slot = EquipmentSlot.Leggings,
                ItemId = mapping.Itemid,
                Metadata = mapping.Metadata
            });

            mapping = ItemMapping.Pe2Pc(spacket.helmet.Id, spacket.helmet.Metadata);
            QueuePacket(new EntityEquipment
            {
                EntityId = (int)spacket.entityId,
                Slot = EquipmentSlot.Helmet,
                ItemId = mapping.Itemid,
                Metadata = mapping.Metadata
            });
        }

        private void PocketEditionClient_OnTileEntityData(McpeTileEntityData packet)
        {
            var compound = packet.namedtag.NbtFile.RootTag;

            var nbtTag = compound.Get("id");
            if (nbtTag != null && nbtTag.StringValue == "Sign")
            {
                Sign s = new Sign
                {
                    Text1 = compound["Text1"].StringValue,
                    Text2 = compound["Text2"].StringValue,
                    Text3 = compound["Text3"].StringValue,
                    Text4 = compound["Text4"].StringValue
                };
                var x = compound["x"].IntValue;
                var y = compound["y"].IntValue;
                var z = compound["z"].IntValue;
                s.Coordinates = new BlockCoordinates(x, y, z);

                QueuePacket(new Updatesign
                {
                    Line1 = JsonConvert.SerializeObject(new ChatObject(s.Text1)),
                    Line2 = JsonConvert.SerializeObject(new ChatObject(s.Text2)),
                    Line3 = JsonConvert.SerializeObject(new ChatObject(s.Text3)),
                    Line4 = JsonConvert.SerializeObject(new ChatObject(s.Text4)),
                    Location = new Vector3(packet.x, packet.y, packet.z)
                });
            }
            else if (nbtTag != null && nbtTag.StringValue == "Skull")
            {
                UpdateBlockEntity update = new UpdateBlockEntity
                {
                    Action = 4, //Update mob head
                    Data = compound,
                    Location = new Vector3(packet.x, packet.y, packet.z)
                };
                QueuePacket(update);
            }
        }

        private void PocketEditionClient_OnRespawn(McpeRespawn packet)
        {
            QueuePacket(new PlayerPositionAndLook
            {
                Flags = 0,
                Pitch = 0,
                Yaw = 0,
                X = packet.x,
                Y = packet.y,
                Z = packet.z,
                TeleportID = 0
            });
        }

        private void PocketEditionClient_OnAnimation(McpeAnimate packet)
        {
            var actionid = packet.actionId;
            if (actionid == 1) actionid = 0;

            QueuePacket(new Animation
            {
                AnimationId = actionid,
                TargetEntity = (int)packet.entityId
            });
        }

        private void PocketEditionClient_OnBlockUpdate(McpeUpdateBlock packet)
        {
            if (packet.blocks.Count > 1)
            {
                var cx = packet.blocks[0].Coordinates.X >> 4;
                var cz = packet.blocks[0].Coordinates.Z >> 4;
                QueuePacket(new MultiBlockChange
                {
                    ChunkX = cx,
                    ChunkZ = cz,
                    Blocks = packet.blocks
                });
                return;
            }

            QueuePacket(new Blockchange
            {
                Location = packet.blocks[0].Coordinates,
                BlockId = packet.blocks[0].Id << 4 | (packet.blocks[0].Metadata & 15)
            });
        }
        private void PocketEditionClient_OnPlayerRemoval(long entityId, UUID clientUuid)
        {
            QueuePacket(new Destroyentities
            {
                Count = 1,
                EntityId = (int)entityId
            });
        }

        private void PocketEditionClient_OnEntityAdd(McpeAddEntity message)
        {
            if (message.entityType == 64 && message.metadata.Contains(15) &&
                ((MetadataByte) message.metadata[15]).Value == 1) //Hologram text (OMG, this is so cheaty)
            {
                string displayName = ((MetadataString) message.metadata[2]).Value;
                if (string.IsNullOrEmpty(displayName)) return;

                message.y -= 2.3f;
                var msg = displayName.Replace("\n", " "); //PC does not support the newline characther

                var entityId = (int) message.entityId;

                QueuePacket(new SpawnObject
                {
                    Type = 78,
                    EntityId = entityId,
                    EntityUUID = new UUID(Guid.NewGuid()).ToString(),
                    VelocityX = 0,
                    VelocityY = 0,
                    VelocityZ = 0,
                    X = message.x,
                    Y = message.y,
                    Z = message.z,
                    Yaw = message.yaw.ToAngle(),
                    Pitch = message.pitch.ToAngle(),
                    Data = 0
                }, true);

                byte newData = 0x20;

                EntityMetadata meta = new EntityMetadata
                {
                    EntityId = entityId
                };

                byte[] metaData;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (MinecraftStream stream = new MinecraftStream(ms))
                    {
                        stream.WriteUInt8(2);
                        stream.WriteUInt8(3);
                        stream.WriteString(msg);

                        stream.WriteUInt8(3);
                        stream.WriteUInt8(6);
                        stream.WriteBoolean(true);

                        stream.WriteUInt8(0);
                        stream.WriteUInt8(0);
                        stream.WriteUInt8(newData);

                        stream.WriteUInt8(0xff);
                    }
                    metaData = ms.ToArray();
                }

                meta.Metadata = metaData;
                QueuePacket(meta, true);
                return;
            }

            var a = EntityIdTranslator.GetEntityId(message.entityType);
            if (!a.Translated) return;

            short speedx = (short)Math.Round(message.speedX * 8000);
            short speedy = (short)Math.Round(message.speedY * 8000);
            short speedz = (short)Math.Round(message.speedZ * 8000);

            if (a.IsMob)
            {
                QueuePacket(new SpawnMob
                {
                    EntityId = (int)message.entityId,
                    EntityUUID = new UUID(Guid.NewGuid()).ToString(),
                    HeadPitch = message.pitch.ToAngle(),
                    Yaw = message.yaw.ToAngle(),
                    Pitch = message.pitch.ToAngle(),
                    metadata = 0xff,
                    Type = a.Id,
                    VelocityX = speedx,
                    VelocityY = speedy,
                    VelocityZ = speedz,
                    X = message.x,
                    Y = message.y,
                    Z = message.z
                }, true);
                return;
            }


			QueuePacket(new SpawnObject
            {
                EntityId = (int)message.entityId,
                EntityUUID = new UUID(Guid.NewGuid()).ToString(),
                Yaw = message.yaw.ToAngle(),
                Pitch = message.pitch.ToAngle(),
                Type = a.Id,
                VelocityX = speedx,
                VelocityY = speedy,
                VelocityZ = speedz,
                X = message.x,
                Y = message.y,
                Z = message.z,
                Data = message.links
            }, true);
        }

        private void PocketEditionClient_OnTimeChanged(int time)
        {
            QueuePacket(new TimeUpdate
            {
                TimeOfDay = time,
                WorldAge = 0
            });
        }

        private void PocketEditionClient_OnServerDifficulty(Difficulty difficulty)
        {
            QueuePacket(new Serverdifficulty
            {
                Difficulty = (byte)difficulty
            });
        }

        private void PocketEditionClient_OnPlayerEquipment(long entityid, Item item)
        {
            if (item == null) return;

            var itemId = item.Id;
            if (itemId == 0) itemId = -1;

            var mapping = ItemMapping.Pe2Pc(itemId, item.Metadata);
            QueuePacket(new EntityEquipment
            {
                EntityId = (int)entityid,
                Slot = EquipmentSlot.MainHand,
                ItemId = mapping.Itemid,
                Metadata = mapping.Metadata
            });
        }

        private void PocketEditionClient_OnPlayerMovement(McpeMovePlayer message)
        {
            if (message.entityId == 0)
            {
                if (_respawning)
                {
                    _respawning = false;
                }

                CurrentPosition.X = message.x;
                CurrentPosition.Y = message.y;
                CurrentPosition.Z = message.z;
                CurrentPosition.HeadYaw = message.headYaw;
                CurrentPosition.Yaw = message.yaw;
                CurrentPosition.Pitch = message.pitch;

                QueuePacket(new PlayerPositionAndLook
                {
                    Flags = 0,
                    Pitch = message.pitch,
                    Yaw = message.yaw,
                    X = message.x,
                    Y = message.y,
                    Z = message.z,
                    TeleportID = 0
                });
            }
            else
            {
                message.y -= 1.62f;

                QueuePacket(new EntityTeleport
                {
                    EntityId = (int)message.entityId,
                    X = message.x,
                    Y = message.y,
                    Z = message.z,
                    Yaw = message.yaw.ToAngle(),
                    Pitch = message.pitch.ToAngle(),
                    OnGround = (message.onGround == 1)
                });

                QueuePacket(new EntityLook
                {
                    EntityId = (int)message.entityId,
                    Yaw = message.yaw.ToAngle(),
                    Pitch = message.pitch.ToAngle(),
                    OnGround = false
                });

                QueuePacket(new Entityheadlook
                {
                    EntityId = (int) message.entityId,
                    Yaw = message.headYaw.ToAngle()
                });
            }
        }

        private void PocketEditionClient_OnSetContainerSlot(McpeContainerSetSlot packet)
        {
            if (_openedInventory != -999 && packet.windowId == _openedInventory)
            {
                QueuePacket(new SetSlot
                {
                    SlotId = packet.slot,
                    Window = packet.windowId,
                    Slot = packet.item
                });
                return;
            }

            bool done = false;
            var item = packet.item;
            short index = packet.slot;

            switch (packet.windowId)
            {
                case 0:
                    if (index <= 8)
                    {
                        index += 36;
                        done = true;
                    }
                    else if (index >= 9 && index <= 35)
                    {
                        done = true;
                    }
                    break;
                case 0x78:
                    index += 5;
                    done = true;
                    break;
                default: //Unknown window id
                    return;
            }

            if (!done) return;

            InventoryManager.SetSlot(index, item, false);
            QueuePacket(new SetSlot
            {
                SlotId = index,
                Window = 0,
                Slot = item
            });

            if (index == 36 + InventoryManager.SelectedSlot)
            {
                SendHandItem();
            }
        }

        private void PocketEditionClient_OnContainerContent(McpeContainerSetContent package)
        {
            if (_openedInventory != -999 && package.windowId == _openedInventory)
            {
                for (short index = 0; index < package.slotData.Count; index++)
                {
                    var i = package.slotData[index];
                    if (i.Id == 0 || i.Id == -1 || i.Count <= 0) continue;

                    QueuePacket(new SetSlot
                    {
                        SlotId = index,
                        Window = package.windowId,
                        Slot = i
                    });
                }
                return;
            }

            //Inventory = 0x00
            if (package.windowId == 0x78) //Armor
            {
                for (int index = 0; index < package.slotData.Count; index++)
                {
                    var i = package.slotData[index];
                    if (i.Id == 0 || i.Id == -1 || i.Count <= 0) continue;

                    QueuePacket(new SetSlot
                    {
                        Slot = i,
                        SlotId = (short)(5 + index)
                    });
                    InventoryManager.SetSlot((short) (5 + index), i, false);
                }
                return;
            }

            if (package.windowId == 0x00) //Inventory
            {
                for (int index = 0; index < package.slotData.Count; index++)
                {
                    var i = package.slotData[index];
                    if (i.Id == 0 || i.Id == -1 || i.Count <= 0) continue;
                    var usedIndex = index;
                    if (usedIndex < 9)
                    {
                        usedIndex += 36;
                    }

                    QueuePacket(new SetSlot
                    {
                        Slot = i,
                        SlotId = (short)usedIndex
                    });
                    InventoryManager.SetSlot((short)(usedIndex), i, false);

                    if (usedIndex == (36 + InventoryManager.SelectedSlot))
                    {
                        SendHandItem();
                    }
                }
            }
        }

        private void PocketEditionClient_OnPlayerAdd(McpeAddPlayer packet)
        {
            SpawnPlayerQueue.Add(new SpawnPlayer
            {
                EntityId = (int)packet.entityId,
                X = packet.x,
                Y = packet.y,
                Z = packet.z,
                Yaw = packet.headYaw.ToAngle(),
                Pitch = packet.pitch.ToAngle(),
                PlayerUUID = packet.uuid.ToString(),
                metadata = 0xff
            });

            SpawnedPlayers.Add((int) packet.entityId, new SpawnedPlayer()
            {
                EntityId = (int) packet.entityId,
                Username = packet.username,
                UUID = packet.uuid.ToString()
            });

            SendPlayerMetadata((int)packet.entityId, packet.metadata);
        }

        private void PocketEditionClient_OnDisconnect(string reason)
        {
            ShouldRead = false;

            SendPacket(new Disconnect
            {
                Reason = JsonConvert.SerializeObject(new ChatObject(reason))
            });
        }

        private void PocketEditionClient_OnChatMessage(string message, string source, MessageType type)
        {
            //0: chat (chat box), 1: system message (chat box), 2: above hotbar 
            byte position = 0;
            if (type == MessageType.Tip)
            {
                position = 2;
            }
            else if (type == MessageType.Popup)
            {
                position = 2;
            }

            foreach (
                var msg in
                    message.Split(new[] { "\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                var text = msg;
                if (!string.IsNullOrEmpty(source))
                {
                    text = string.Format("<{0}> {1}", source, text);
                }

                QueuePacket(new Chatmessage
                {
                    ChatMessage = JsonConvert.SerializeObject(new ChatObject(text)),
                    Position = position
                });
            }
        }

        private void PocketEditionClient_OnSetSpawnPosition(Vector3 location)
        {
            SpawnPosition = location;
        }

        private void PocketEditionClient_OnChunkData(ChunkColumn chunk)
        {
            SendChunkColumn(chunk);
        }

        private void PocketEditionClientOnOnStartGame(GameMode gamemode, Vector3 spawnPoint, long entityId)
        {
            CurrentPosition = new PlayerLocation(spawnPoint);
            _canFly = (gamemode == GameMode.Creative);
            _flying = (gamemode == GameMode.Creative);
            _invulnerable = (gamemode == GameMode.Creative);

            Gamemode = gamemode;
            EntityId = entityId;

            if (!_spawned)
            {
                QueuePacket(new JoinGame
                {
                    Difficulty = 0,
                    Dimension = 0,
                    EntityId = (int)EntityId,
                    Gamemode = (byte)gamemode,
                    LevelType = "default",
                    MaxPlayers = 255,
                    ReducedDebugInfo = false
                });
            }

            if (SpawnPosition != spawnPoint)
            {
                SpawnPosition = spawnPoint;
                QueuePacket(new SpawnPosition
                {
                    Location = SpawnPosition
                });
            }

            SendPlayerAbilities();

            if (!_spawned)
            {
                QueuePacket(new AddPlayerListItem
                {
                    Gamemode = 0,
                    HasDisplayName = false,
                    Name = "\u00A76\u00A7lPocketProxy",
                    NumberOfPlayers = 1,
                    NumberOfProperties = 0,
                    Ping = 0,
                    UUID = PcClientUuid.ToString()
                });

                McpeRequestChunkRadius request = McpeRequestChunkRadius.CreateObject();
                request.chunkRadius = ChunkRadius;
                PeClient.SendPackage(request);
            }
            else
            {
                QueuePacket(new ChangeGamestate
                {
                    Reason = 3,
                    Value = (int)gamemode
                });

                QueuePacket(new Updategamemode
                {
                    UUID = PcClientUuid.ToString(),
                    Gamemode = (int)gamemode,
                    NumberOfPlayers = 1
                });
            }

            QueuePacket(new PlayerPositionAndLook
            {
                Flags = 0,
                Pitch = CurrentPosition.Pitch,
                Yaw = CurrentPosition.Yaw,
                X = SpawnPosition.X,
                Y = SpawnPosition.Y,
                Z = SpawnPosition.Z,
                TeleportID = 0
            });

            _spawned = true;
        }

        private void SendPlayerMetadata(int entityid, MetadataDictionary metadata)
        {
            if (!metadata.Contains(0)) return;

            var data = (MetadataByte)metadata[0];
           // var air = (MetadataShort)metadata[1];
           // var silent = (MetadataByte)metadata[4];

            bool fire = ByteHelper.GetBit(data.Value, 0);
            bool sneaking = ByteHelper.GetBit(data.Value, 1);
            bool invisible = ByteHelper.GetBit(data.Value, 5);

            byte newData = 0;
            ByteHelper.SetBit(ref newData, 0x00, fire);
            ByteHelper.SetBit(ref newData, 0x01, sneaking);
            ByteHelper.SetBit(ref newData, 0x19, invisible);

            byte[] metaData;
            using (MemoryStream ms = new MemoryStream())
            {
                using (MinecraftStream stream = new MinecraftStream(ms))
                {
                    stream.WriteUInt8(0);
                    stream.WriteUInt8(0);
                    stream.WriteUInt8(newData);

                    stream.WriteUInt8(0xff);
                }
                metaData = ms.ToArray();
            }

            QueuePacket(new EntityMetadata
            {
                EntityId = entityid,
                Metadata = metaData
            });
        }
    }
}
