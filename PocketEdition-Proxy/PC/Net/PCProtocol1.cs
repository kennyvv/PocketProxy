
//
// WARNING: T4 GENERATED CODE - DO NOT EDIT
// Please edit "Protocol.xml" instead
// 

// ReSharper disable InconsistentNaming
using PocketProxy.PC.Utils;
using PocketProxy.PC.Objects;
using MiNET.Utils;
using System.Numerics;

namespace PocketProxy.PC.Net
{
	using Serverbound;
	public class PacketFactory
	{
		public static Packet CreatePacket(int packetId, byte[] buffer, PacketState packetState)
		{
			Packet packet; 
			switch(packetState)
			{
				case PacketState.Handshake:
					switch (packetId)
					{
						case 0x00:
								packet = new Handshake();
								packet.Read(buffer);
								return packet;
					}
					break;
					case PacketState.Status:
						switch (packetId)
						{
							case 0x00:
								packet = new Request();
								packet.Read(buffer);
								return packet;
							case 0x01:
								packet = new Ping();
								packet.Read(buffer);
								return packet;
						}
					break;
					case PacketState.Login:
						switch (packetId)
						{
							case 0x00:
								packet = new Loginstart();
								packet.Read(buffer);
								return packet;
							case 0x01:
								packet = new EncryptionResponse();
								packet.Read(buffer);
								return packet;
						}
						break;
					case PacketState.Play:
						switch (packetId)
						{
							case 0x04:
								packet = new ClientSettings();
								packet.Read(buffer);
								return packet;							
							case 0x02:
								packet = new Chatmessage();
								packet.Read(buffer);
								return packet;							
							case 0x0D:
								packet = new PlayerPositionAndLook();
								packet.Read(buffer);
								return packet;							
							case 0x0C:
								packet = new PlayerPosition();
								packet.Read(buffer);
								return packet;							
							case 0x0E:
								packet = new PlayerLook();
								packet.Read(buffer);
								return packet;							
							case 0x0B:
								packet = new KeepAlive();
								packet.Read(buffer);
								return packet;							
							case 0x1C:
								packet = new Playerblockplacement();
								packet.Read(buffer);
								return packet;							
							case 0x13:
								packet = new Playerdigging();
								packet.Read(buffer);
								return packet;							
							case 0x03:
								packet = new Clientstatus();
								packet.Read(buffer);
								return packet;							
							case 0x1A:
								packet = new Animation();
								packet.Read(buffer);
								return packet;							
							case 0x14:
								packet = new EntityAction();
								packet.Read(buffer);
								return packet;							
							case 0x17:
								packet = new HeldItemChange();
								packet.Read(buffer);
								return packet;							
							case 0x08:
								packet = new CloseWindow();
								packet.Read(buffer);
								return packet;							
							case 0x07:
								packet = new Serverbound.ClickWindow();
								packet.Read(buffer);
								return packet;
							case 0x18:
								packet = new Serverbound.CreativeInventoryAction();
								packet.Read(buffer);
								return packet;
							case 0x0A:
								packet = new Serverbound.UseEntity();
								packet.Read(buffer);
								return packet;
						}
						break;
					}
			return null;
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Handshake : Packet
	{
		public int ProtocolVersion;
		public string ServerAddress;
		public ushort ServerPort;
		public PacketState NextState;
		public Handshake()
		{
			PacketId = 0x00;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(ProtocolVersion);
			stream.WriteString(ServerAddress);
			stream.WriteUShort(ServerPort);
			stream.WritePacketState(NextState);
		}

		public override void Read(MinecraftStream stream)
		{
			ProtocolVersion = stream.ReadVarInt();
			ServerAddress = stream.ReadString();
			ServerPort = stream.ReadUShort();
			NextState = stream.ReadPacketState();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class ClientSettings : Packet
	{
		public string Locale;
		public byte ViewDistance;
		public int ChatMode;
		public bool ChatColors;
		public byte DisplayedSkinParts;
		public int MainHand;
		public ClientSettings()
		{
			PacketId = 0x04;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteString(Locale);
			stream.WriteUInt8(ViewDistance);
			stream.WriteVarInt(ChatMode);
			stream.WriteBoolean(ChatColors);
			stream.WriteUInt8(DisplayedSkinParts);
			stream.WriteVarInt(MainHand);
		}

		public override void Read(MinecraftStream stream)
		{
			Locale = stream.ReadString();
			ViewDistance = stream.ReadUInt8();
			ChatMode = stream.ReadVarInt();
			ChatColors = stream.ReadBoolean();
			DisplayedSkinParts = stream.ReadUInt8();
			MainHand = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Request : Packet
	{
		public Request()
		{
			PacketId = 0x00;
		}

		public override void Write(MinecraftStream stream)
		{
		}

		public override void Read(MinecraftStream stream)
		{
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Ping : Packet
	{
		public long Payload;
		public Ping()
		{
			PacketId = 0x01;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteLong(Payload);
		}

		public override void Read(MinecraftStream stream)
		{
			Payload = stream.ReadLong();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Chatmessage : Packet
	{
		public string ChatMessage;
		public Chatmessage()
		{
			PacketId = 0x02;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteString(ChatMessage);
		}

		public override void Read(MinecraftStream stream)
		{
			ChatMessage = stream.ReadString();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Loginstart : Packet
	{
		public string Name;
		public Loginstart()
		{
			PacketId = 0x00;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteString(Name);
		}

		public override void Read(MinecraftStream stream)
		{
			Name = stream.ReadString();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class PlayerPositionAndLook : Packet
	{
		public double X;
		public double Y;
		public double Z;
		public float Yaw;
		public float Pitch;
		public bool Onground;
		public PlayerPositionAndLook()
		{
			PacketId = 0x0d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteFloat(Yaw);
			stream.WriteFloat(Pitch);
			stream.WriteBoolean(Onground);
		}

		public override void Read(MinecraftStream stream)
		{
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Z = stream.ReadDouble();
			Yaw = stream.ReadFloat();
			Pitch = stream.ReadFloat();
			Onground = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class ChangeGamestate : Packet
	{
		public byte Reason;
		public float Value;
		public ChangeGamestate()
		{
			PacketId = 0x1e;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteUInt8(Reason);
			stream.WriteFloat(Value);
		}

		public override void Read(MinecraftStream stream)
		{
			Reason = stream.ReadUInt8();
			Value = stream.ReadFloat();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class PlayerPosition : Packet
	{
		public double X;
		public double Y;
		public double Z;
		public bool Onground;
		public PlayerPosition()
		{
			PacketId = 0x0c;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteBoolean(Onground);
		}

		public override void Read(MinecraftStream stream)
		{
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Z = stream.ReadDouble();
			Onground = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class PlayerLook : Packet
	{
		public float Yaw;
		public float Pitch;
		public bool Onground;
		public PlayerLook()
		{
			PacketId = 0x0e;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteFloat(Yaw);
			stream.WriteFloat(Pitch);
			stream.WriteBoolean(Onground);
		}

		public override void Read(MinecraftStream stream)
		{
			Yaw = stream.ReadFloat();
			Pitch = stream.ReadFloat();
			Onground = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Response : Packet
	{
		public StatusResponse Status;
		public Response()
		{
			PacketId = 0x00;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteStatusResponse(Status);
		}

		public override void Read(MinecraftStream stream)
		{
			Status = stream.ReadStatusResponse();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class LoginSuccess : Packet
	{
		public string UUID;
		public string Username;
		public LoginSuccess()
		{
			PacketId = 0x02;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteString(UUID);
			stream.WriteString(Username);
		}

		public override void Read(MinecraftStream stream)
		{
			UUID = stream.ReadString();
			Username = stream.ReadString();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class SetCompression : Packet
	{
		public int Threshold;
		public SetCompression()
		{
			PacketId = 0x03;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Threshold);
		}

		public override void Read(MinecraftStream stream)
		{
			Threshold = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class ChunkData : Packet
	{
		public int ChunkX;
		public int ChunkZ;
		public bool GroundUpContinuous;
		public int PrimaryBitmask;
		public int SizeOfData;
		public byte[] Data;
		public byte[] Biome;
		public ChunkData()
		{
			PacketId = 0x20;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteInt(ChunkX);
			stream.WriteInt(ChunkZ);
			stream.WriteBoolean(GroundUpContinuous);
			stream.WriteVarInt(PrimaryBitmask);
			stream.WriteVarInt(SizeOfData);
			stream.WriteBytes(Data);
			stream.WriteBytes(Biome);
		}

		public override void Read(MinecraftStream stream)
		{
			ChunkX = stream.ReadInt();
			ChunkZ = stream.ReadInt();
			GroundUpContinuous = stream.ReadBoolean();
			PrimaryBitmask = stream.ReadVarInt();
			SizeOfData = stream.ReadVarInt();
			Data = stream.ReadBytes();
			Biome = stream.ReadBytes();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class SpawnPosition : Packet
	{
		public Vector3 Location;
		public SpawnPosition()
		{
			PacketId = 0x43;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WritePosition(Location);
		}

		public override void Read(MinecraftStream stream)
		{
			Location = stream.ReadPosition();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class PlayerPositionAndLook : Packet
	{
		public double X;
		public double Y;
		public double Z;
		public float Yaw;
		public float Pitch;
		public byte Flags;
		public int TeleportID;
		public PlayerPositionAndLook()
		{
			PacketId = 0x2e;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteFloat(Yaw);
			stream.WriteFloat(Pitch);
			stream.WriteUInt8(Flags);
			stream.WriteVarInt(TeleportID);
		}

		public override void Read(MinecraftStream stream)
		{
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Z = stream.ReadDouble();
			Yaw = stream.ReadFloat();
			Pitch = stream.ReadFloat();
			Flags = stream.ReadUInt8();
			TeleportID = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class JoinGame : Packet
	{
		public int EntityId;
		public byte Gamemode;
		public int Dimension;
		public byte Difficulty;
		public byte MaxPlayers;
		public string LevelType;
		public bool ReducedDebugInfo;
		public JoinGame()
		{
			PacketId = 0x23;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteInt(EntityId);
			stream.WriteUInt8(Gamemode);
			stream.WriteInt(Dimension);
			stream.WriteUInt8(Difficulty);
			stream.WriteUInt8(MaxPlayers);
			stream.WriteString(LevelType);
			stream.WriteBoolean(ReducedDebugInfo);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadInt();
			Gamemode = stream.ReadUInt8();
			Dimension = stream.ReadInt();
			Difficulty = stream.ReadUInt8();
			MaxPlayers = stream.ReadUInt8();
			LevelType = stream.ReadString();
			ReducedDebugInfo = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class PlayerAbilities : Packet
	{
		public byte Flags;
		public float FlyingSpeed;
		public float FieldOfViewModifier;
		public PlayerAbilities()
		{
			PacketId = 0x2b;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteUInt8(Flags);
			stream.WriteFloat(FlyingSpeed);
			stream.WriteFloat(FieldOfViewModifier);
		}

		public override void Read(MinecraftStream stream)
		{
			Flags = stream.ReadUInt8();
			FlyingSpeed = stream.ReadFloat();
			FieldOfViewModifier = stream.ReadFloat();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class KeepAlive : Packet
	{
		public int KeepAliveID;
		public KeepAlive()
		{
			PacketId = 0x1f;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(KeepAliveID);
		}

		public override void Read(MinecraftStream stream)
		{
			KeepAliveID = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class KeepAlive : Packet
	{
		public int KeepAliveID;
		public KeepAlive()
		{
			PacketId = 0x0b;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(KeepAliveID);
		}

		public override void Read(MinecraftStream stream)
		{
			KeepAliveID = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class UnloadChunk : Packet
	{
		public int ChunkX;
		public int ChunkZ;
		public UnloadChunk()
		{
			PacketId = 0x1d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteInt(ChunkX);
			stream.WriteInt(ChunkZ);
		}

		public override void Read(MinecraftStream stream)
		{
			ChunkX = stream.ReadInt();
			ChunkZ = stream.ReadInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Chatmessage : Packet
	{
		public string ChatMessage;
		public byte Position;
		public Chatmessage()
		{
			PacketId = 0x0f;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteString(ChatMessage);
			stream.WriteUInt8(Position);
		}

		public override void Read(MinecraftStream stream)
		{
			ChatMessage = stream.ReadString();
			Position = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Pong : Packet
	{
		public long Payload;
		public Pong()
		{
			PacketId = 0x01;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteLong(Payload);
		}

		public override void Read(MinecraftStream stream)
		{
			Payload = stream.ReadLong();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class AddPlayerListItem : Packet
	{
		public int Action=0;
		public int NumberOfPlayers;
		public string UUID;
		public string Name;
		public int NumberOfProperties=0;
		public int Gamemode;
		public int Ping;
		public bool HasDisplayName=false;
		public AddPlayerListItem()
		{
			PacketId = 0x2d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Action);
			stream.WriteVarInt(NumberOfPlayers);
			stream.WriteUUID(UUID);
			stream.WriteString(Name);
			stream.WriteVarInt(NumberOfProperties);
			stream.WriteVarInt(Gamemode);
			stream.WriteVarInt(Ping);
			stream.WriteBoolean(HasDisplayName);
		}

		public override void Read(MinecraftStream stream)
		{
			Action = stream.ReadVarInt();
			NumberOfPlayers = stream.ReadVarInt();
			UUID = stream.ReadUUID();
			Name = stream.ReadString();
			NumberOfProperties = stream.ReadVarInt();
			Gamemode = stream.ReadVarInt();
			Ping = stream.ReadVarInt();
			HasDisplayName = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class UpdateDisplayName : Packet
	{
		public int Action=3;
		public int NumberOfPlayers;
		public string UUID;
		public bool HasDisplayName=true;
		public string DisplayName;
		public UpdateDisplayName()
		{
			PacketId = 0x2d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Action);
			stream.WriteVarInt(NumberOfPlayers);
			stream.WriteUUID(UUID);
			stream.WriteBoolean(HasDisplayName);
			stream.WriteString(DisplayName);
		}

		public override void Read(MinecraftStream stream)
		{
			Action = stream.ReadVarInt();
			NumberOfPlayers = stream.ReadVarInt();
			UUID = stream.ReadUUID();
			HasDisplayName = stream.ReadBoolean();
			DisplayName = stream.ReadString();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class AddPlayerListItemWithSkin : Packet
	{
		public int Action=0;
		public int NumberOfPlayers;
		public string UUID;
		public string Name;
		public int NumberOfProperties=1;
		public string PropertieName="textures";
		public string PropertieValue;
		public bool PropertieSigned=false;
		public int Gamemode;
		public int Ping;
		public bool HasDisplayName=false;
		public AddPlayerListItemWithSkin()
		{
			PacketId = 0x2d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Action);
			stream.WriteVarInt(NumberOfPlayers);
			stream.WriteUUID(UUID);
			stream.WriteString(Name);
			stream.WriteVarInt(NumberOfProperties);
			stream.WriteString(PropertieName);
			stream.WriteString(PropertieValue);
			stream.WriteBoolean(PropertieSigned);
			stream.WriteVarInt(Gamemode);
			stream.WriteVarInt(Ping);
			stream.WriteBoolean(HasDisplayName);
		}

		public override void Read(MinecraftStream stream)
		{
			Action = stream.ReadVarInt();
			NumberOfPlayers = stream.ReadVarInt();
			UUID = stream.ReadUUID();
			Name = stream.ReadString();
			NumberOfProperties = stream.ReadVarInt();
			PropertieName = stream.ReadString();
			PropertieValue = stream.ReadString();
			PropertieSigned = stream.ReadBoolean();
			Gamemode = stream.ReadVarInt();
			Ping = stream.ReadVarInt();
			HasDisplayName = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Playerblockplacement : Packet
	{
		public long Location;
		public int Face;
		public int Hand;
		public byte CursorPositionX;
		public byte CursorPositionY;
		public byte CursorPositionZ;
		public Playerblockplacement()
		{
			PacketId = 0x1c;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteLong(Location);
			stream.WriteVarInt(Face);
			stream.WriteVarInt(Hand);
			stream.WriteUInt8(CursorPositionX);
			stream.WriteUInt8(CursorPositionY);
			stream.WriteUInt8(CursorPositionZ);
		}

		public override void Read(MinecraftStream stream)
		{
			Location = stream.ReadLong();
			Face = stream.ReadVarInt();
			Hand = stream.ReadVarInt();
			CursorPositionX = stream.ReadUInt8();
			CursorPositionY = stream.ReadUInt8();
			CursorPositionZ = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Playerdigging : Packet
	{
		public byte Status;
		public long Location;
		public byte Face;
		public Playerdigging()
		{
			PacketId = 0x13;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteUInt8(Status);
			stream.WriteLong(Location);
			stream.WriteUInt8(Face);
		}

		public override void Read(MinecraftStream stream)
		{
			Status = stream.ReadUInt8();
			Location = stream.ReadLong();
			Face = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class UpdatePlayerLatency : Packet
	{
		public int Action=2;
		public int NumberOfPlayers;
		public string UUID;
		public int Ping;
		public UpdatePlayerLatency()
		{
			PacketId = 0x2d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Action);
			stream.WriteVarInt(NumberOfPlayers);
			stream.WriteUUID(UUID);
			stream.WriteVarInt(Ping);
		}

		public override void Read(MinecraftStream stream)
		{
			Action = stream.ReadVarInt();
			NumberOfPlayers = stream.ReadVarInt();
			UUID = stream.ReadUUID();
			Ping = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class RemovePlayerListItem : Packet
	{
		public int Action=4;
		public int NumberOfPlayers;
		public string UUID;
		public RemovePlayerListItem()
		{
			PacketId = 0x2d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Action);
			stream.WriteVarInt(NumberOfPlayers);
			stream.WriteUUID(UUID);
		}

		public override void Read(MinecraftStream stream)
		{
			Action = stream.ReadVarInt();
			NumberOfPlayers = stream.ReadVarInt();
			UUID = stream.ReadUUID();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Updategamemode : Packet
	{
		public int Action=1;
		public int NumberOfPlayers;
		public string UUID;
		public int Gamemode;
		public Updategamemode()
		{
			PacketId = 0x2d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Action);
			stream.WriteVarInt(NumberOfPlayers);
			stream.WriteUUID(UUID);
			stream.WriteVarInt(Gamemode);
		}

		public override void Read(MinecraftStream stream)
		{
			Action = stream.ReadVarInt();
			NumberOfPlayers = stream.ReadVarInt();
			UUID = stream.ReadUUID();
			Gamemode = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class SetExperience : Packet
	{
		public float ExperienceBar;
		public int Level;
		public int TotalExperience;
		public SetExperience()
		{
			PacketId = 0x3d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteFloat(ExperienceBar);
			stream.WriteVarInt(Level);
			stream.WriteVarInt(TotalExperience);
		}

		public override void Read(MinecraftStream stream)
		{
			ExperienceBar = stream.ReadFloat();
			Level = stream.ReadVarInt();
			TotalExperience = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class SpawnPlayer : Packet
	{
		public int EntityId;
		public string PlayerUUID;
		public double X;
		public double Y;
		public double Z;
		public byte Yaw;
		public byte Pitch;
		public byte metadata=0xff;
		public SpawnPlayer()
		{
			PacketId = 0x05;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteUUID(PlayerUUID);
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteUInt8(Yaw);
			stream.WriteUInt8(Pitch);
			stream.WriteUInt8(metadata);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadVarInt();
			PlayerUUID = stream.ReadUUID();
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Z = stream.ReadDouble();
			Yaw = stream.ReadUInt8();
			Pitch = stream.ReadUInt8();
			metadata = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class EncryptionRequest : Packet
	{
		public string ServerID;
		public int PublicKeyLength;
		public byte[] PublicKey;
		public int VerifyTokenLength;
		public byte[] VerifyToken;
		public EncryptionRequest()
		{
			PacketId = 0x01;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteString(ServerID);
			stream.WriteVarInt(PublicKeyLength);
			stream.WriteBytes(PublicKey);
			stream.WriteVarInt(VerifyTokenLength);
			stream.WriteBytes(VerifyToken);
		}

		public override void Read(MinecraftStream stream)
		{
			ServerID = stream.ReadString();
			PublicKeyLength = stream.ReadVarInt();
			PublicKey = stream.ReadBytes();
			VerifyTokenLength = stream.ReadVarInt();
			VerifyToken = stream.ReadBytes();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Blockchange : Packet
	{
		public Vector3 Location;
		public int BlockId;
		public Blockchange()
		{
			PacketId = 0x0b;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WritePosition(Location);
			stream.WriteVarInt(BlockId);
		}

		public override void Read(MinecraftStream stream)
		{
			Location = stream.ReadPosition();
			BlockId = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Updatesign : Packet
	{
		public Vector3 Location;
		public string Line1;
		public string Line2;
		public string Line3;
		public string Line4;
		public Updatesign()
		{
			PacketId = 0x46;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WritePosition(Location);
			stream.WriteString(Line1);
			stream.WriteString(Line2);
			stream.WriteString(Line3);
			stream.WriteString(Line4);
		}

		public override void Read(MinecraftStream stream)
		{
			Location = stream.ReadPosition();
			Line1 = stream.ReadString();
			Line2 = stream.ReadString();
			Line3 = stream.ReadString();
			Line4 = stream.ReadString();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class EntityTeleport : Packet
	{
		public int EntityId;
		public double X;
		public double Y;
		public double Z;
		public byte Yaw;
		public byte Pitch;
		public bool OnGround;
		public EntityTeleport()
		{
			PacketId = 0x4a;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteUInt8(Yaw);
			stream.WriteUInt8(Pitch);
			stream.WriteBoolean(OnGround);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadVarInt();
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Z = stream.ReadDouble();
			Yaw = stream.ReadUInt8();
			Pitch = stream.ReadUInt8();
			OnGround = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Entityheadlook : Packet
	{
		public int EntityId;
		public byte Yaw;
		public Entityheadlook()
		{
			PacketId = 0x34;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteUInt8(Yaw);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadVarInt();
			Yaw = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class EntityLook : Packet
	{
		public int EntityId;
		public byte Yaw;
		public byte Pitch;
		public bool OnGround;
		public EntityLook()
		{
			PacketId = 0x27;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteUInt8(Yaw);
			stream.WriteUInt8(Pitch);
			stream.WriteBoolean(OnGround);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadVarInt();
			Yaw = stream.ReadUInt8();
			Pitch = stream.ReadUInt8();
			OnGround = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Entitylookandrelativemove : Packet
	{
		public int EntityId;
		public byte X;
		public byte Y;
		public byte Z;
		public byte Yaw;
		public byte Pitch;
		public bool OnGround;
		public Entitylookandrelativemove()
		{
			PacketId = 0x26;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteUInt8(X);
			stream.WriteUInt8(Y);
			stream.WriteUInt8(Z);
			stream.WriteUInt8(Yaw);
			stream.WriteUInt8(Pitch);
			stream.WriteBoolean(OnGround);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadVarInt();
			X = stream.ReadUInt8();
			Y = stream.ReadUInt8();
			Z = stream.ReadUInt8();
			Yaw = stream.ReadUInt8();
			Pitch = stream.ReadUInt8();
			OnGround = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Destroyentities : Packet
	{
		public int Count=1;
		public int EntityId;
		public Destroyentities()
		{
			PacketId = 0x30;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Count);
			stream.WriteVarInt(EntityId);
		}

		public override void Read(MinecraftStream stream)
		{
			Count = stream.ReadVarInt();
			EntityId = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Disconnect : Packet
	{
		public string Reason;
		public Disconnect()
		{
			PacketId = 0x1a;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteString(Reason);
		}

		public override void Read(MinecraftStream stream)
		{
			Reason = stream.ReadString();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Serverdifficulty : Packet
	{
		public byte Difficulty;
		public Serverdifficulty()
		{
			PacketId = 0x0d;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteUInt8(Difficulty);
		}

		public override void Read(MinecraftStream stream)
		{
			Difficulty = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Updatehealth : Packet
	{
		public float Health;
		public int Food;
		public float FoodSaturation;
		public Updatehealth()
		{
			PacketId = 0x3e;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteFloat(Health);
			stream.WriteVarInt(Food);
			stream.WriteFloat(FoodSaturation);
		}

		public override void Read(MinecraftStream stream)
		{
			Health = stream.ReadFloat();
			Food = stream.ReadVarInt();
			FoodSaturation = stream.ReadFloat();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class CollectItem : Packet
	{
		public int CollectedEntityId;
		public int CollectorEntityId;
		public CollectItem()
		{
			PacketId = 0x49;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(CollectedEntityId);
			stream.WriteVarInt(CollectorEntityId);
		}

		public override void Read(MinecraftStream stream)
		{
			CollectedEntityId = stream.ReadVarInt();
			CollectorEntityId = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class TimeUpdate : Packet
	{
		public long WorldAge;
		public long TimeOfDay;
		public TimeUpdate()
		{
			PacketId = 0x44;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteLong(WorldAge);
			stream.WriteLong(TimeOfDay);
		}

		public override void Read(MinecraftStream stream)
		{
			WorldAge = stream.ReadLong();
			TimeOfDay = stream.ReadLong();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class SpawnMob : Packet
	{
		public int EntityId;
		public string EntityUUID;
		public byte Type;
		public double X;
		public double Y;
		public double Z;
		public byte Yaw;
		public byte Pitch;
		public byte HeadPitch;
		public short VelocityX;
		public short VelocityY;
		public short VelocityZ;
		public byte metadata=0xff;
		public SpawnMob()
		{
			PacketId = 0x03;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteUUID(EntityUUID);
			stream.WriteUInt8(Type);
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteUInt8(Yaw);
			stream.WriteUInt8(Pitch);
			stream.WriteUInt8(HeadPitch);
			stream.WriteShort(VelocityX);
			stream.WriteShort(VelocityY);
			stream.WriteShort(VelocityZ);
			stream.WriteUInt8(metadata);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadVarInt();
			EntityUUID = stream.ReadUUID();
			Type = stream.ReadUInt8();
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Z = stream.ReadDouble();
			Yaw = stream.ReadUInt8();
			Pitch = stream.ReadUInt8();
			HeadPitch = stream.ReadUInt8();
			VelocityX = stream.ReadShort();
			VelocityY = stream.ReadShort();
			VelocityZ = stream.ReadShort();
			metadata = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class OpenWindow : Packet
	{
		public byte WindowId;
		public string Type;
		public string Title;
		public byte Slots;
		public OpenWindow()
		{
			PacketId = 0x13;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteUInt8(WindowId);
			stream.WriteString(Type);
			stream.WriteString(Title);
			stream.WriteUInt8(Slots);
		}

		public override void Read(MinecraftStream stream)
		{
			WindowId = stream.ReadUInt8();
			Type = stream.ReadString();
			Title = stream.ReadString();
			Slots = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class BlockAction : Packet
	{
		public Vector3 Location;
		public byte Byte1;
		public byte Byte2;
		public int BlockType;
		public BlockAction()
		{
			PacketId = 0x0a;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WritePosition(Location);
			stream.WriteUInt8(Byte1);
			stream.WriteUInt8(Byte2);
			stream.WriteVarInt(BlockType);
		}

		public override void Read(MinecraftStream stream)
		{
			Location = stream.ReadPosition();
			Byte1 = stream.ReadUInt8();
			Byte2 = stream.ReadUInt8();
			BlockType = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Clientstatus : Packet
	{
		public int ActionId;
		public Clientstatus()
		{
			PacketId = 0x03;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(ActionId);
		}

		public override void Read(MinecraftStream stream)
		{
			ActionId = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class Animation : Packet
	{
		public int TargetEntity;
		public byte AnimationId;
		public Animation()
		{
			PacketId = 0x06;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(TargetEntity);
			stream.WriteUInt8(AnimationId);
		}

		public override void Read(MinecraftStream stream)
		{
			TargetEntity = stream.ReadVarInt();
			AnimationId = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class Animation : Packet
	{
		public int Hand;
		public Animation()
		{
			PacketId = 0x1a;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(Hand);
		}

		public override void Read(MinecraftStream stream)
		{
			Hand = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class EntityAction : Packet
	{
		public int EntityId;
		public int ActionId;
		public EntityAction()
		{
			PacketId = 0x14;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteVarInt(ActionId);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadVarInt();
			ActionId = stream.ReadVarInt();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class HeldItemChange : Packet
	{
		public short Slot;
		public HeldItemChange()
		{
			PacketId = 0x17;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteShort(Slot);
		}

		public override void Read(MinecraftStream stream)
		{
			Slot = stream.ReadShort();
		}
	}
}

namespace PocketProxy.PC.Net.Serverbound
{
	public class CloseWindow : Packet
	{
		public byte WindowId;
		public CloseWindow()
		{
			PacketId = 0x08;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteUInt8(WindowId);
		}

		public override void Read(MinecraftStream stream)
		{
			WindowId = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class CloseWindow : Packet
	{
		public byte WindowId;
		public CloseWindow()
		{
			PacketId = 0x12;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteUInt8(WindowId);
		}

		public override void Read(MinecraftStream stream)
		{
			WindowId = stream.ReadUInt8();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class EntityVelocity : Packet
	{
		public int EntityId;
		public short VelocityX;
		public short VelocityY;
		public short VelocityZ;
		public EntityVelocity()
		{
			PacketId = 0x3b;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteShort(VelocityX);
			stream.WriteShort(VelocityY);
			stream.WriteShort(VelocityZ);
		}

		public override void Read(MinecraftStream stream)
		{
			EntityId = stream.ReadVarInt();
			VelocityX = stream.ReadShort();
			VelocityY = stream.ReadShort();
			VelocityZ = stream.ReadShort();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class ConfirmTransaction : Packet
	{
		public byte WindowId;
		public short ActionNumber;
		public bool Accepted;
		public ConfirmTransaction()
		{
			PacketId = 0x11;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteUInt8(WindowId);
			stream.WriteShort(ActionNumber);
			stream.WriteBoolean(Accepted);
		}

		public override void Read(MinecraftStream stream)
		{
			WindowId = stream.ReadUInt8();
			ActionNumber = stream.ReadShort();
			Accepted = stream.ReadBoolean();
		}
	}
}

namespace PocketProxy.PC.Net.Clientbound
{
	public class PlayerListHeaderFooter : Packet
	{
		public string Header;
		public string Footer;
		public PlayerListHeaderFooter()
		{
			PacketId = 0x48;
		}

		public override void Write(MinecraftStream stream)
		{
			stream.WriteString(Header);
			stream.WriteString(Footer);
		}

		public override void Read(MinecraftStream stream)
		{
			Header = stream.ReadString();
			Footer = stream.ReadString();
		}
	}
}


