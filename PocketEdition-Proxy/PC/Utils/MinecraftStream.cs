using System;
using System.IO;
using System.Numerics;
using System.Text;
using fNbt;
using MiNET.Utils;
using Newtonsoft.Json;
using PocketProxy.PC.Objects;

namespace PocketProxy.PC.Utils
{
    /// <summary>
    ///     Credits to the Craft.NET Project
    ///     Copyright (c) 2011-2013 Drew DeVault
    /// </summary>
    public class MinecraftStream : Stream
    {
        public MinecraftStream(Stream baseStream)
        {
            BaseStream = baseStream;
            StringEncoding = Encoding.UTF8;
        }

        public Encoding StringEncoding { get; set; }

        public Stream BaseStream { get; set; }

        public override bool CanRead
        {
            get { return BaseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return BaseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return BaseStream.CanWrite; }
        }

        public override long Length
        {
            get { return BaseStream.Length; }
        }

        public override long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        public byte[] ToArray()
        {
            return ToByteArray(this);
        }

        private byte[] ToByteArray(Stream stream)
        {
            var length = stream.Length > int.MaxValue ? int.MaxValue : Convert.ToInt32(stream.Length);
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }

        public static int GetVarIntLength(int _value)
        {
            var value = (uint)_value;
            var length = 0;
            while (true)
            {
                length++;
                if ((value & 0xFFFFFF80u) == 0)
                    break;
                value >>= 7;
            }
            return length;
        }

        /// <summary>
        ///     Reads a variable-length integer from the stream.
        /// </summary>
        public int ReadVarInt()
        {
            uint result = 0;
            var length = 0;
            while (true)
            {
                var current = ReadUInt8();
                result |= (current & 0x7Fu) << length++ * 7;
                if (length > 5)
                    throw new InvalidDataException("VarInt may not be longer than 28 bits.");
                if ((current & 0x80) != 128)
                    break;
            }
            return (int)result;
        }

        /// <summary>
        ///     Reads a variable-length integer from the stream.
        /// </summary>
        /// <param name="length">The actual length, in bytes, of the integer.</param>
        public int ReadVarInt(out int length)
        {
            uint result = 0;
            length = 0;
            while (true)
            {
                var current = ReadUInt8();
                result |= (current & 0x7Fu) << length++ * 7;
                if (length > 5)
                    throw new InvalidDataException("VarInt may not be longer than 60 bits.");
                if ((current & 0x80) != 128)
                    break;
            }
            return (int)result;
        }

        /// <summary>
        ///     Writes a variable-length integer to the stream.
        /// </summary>
        public void WriteVarInt(int value)
        {
            var val = (uint)value;
            while (true)
            {
                if ((val & 0xFFFFFF80u) == 0)
                {
                    WriteUInt8((byte)val);
                    break;
                }
                WriteUInt8((byte)(val & 0x7F | 0x80));
                val >>= 7;
            }
        }

        public byte ReadUInt8()
        {
            var value = BaseStream.ReadByte();
            if (value == -1)
                throw new EndOfStreamException();
            return (byte)value;
        }

        public void WriteUInt8(byte value)
        {
            WriteByte(value);
        }

        public sbyte ReadInt8()
        {
            return (sbyte)ReadUInt8();
        }

        public void WriteInt8(sbyte value)
        {
            WriteUInt8((byte)value);
        }

        public ushort ReadUShort()
        {
            return (ushort)(
                (ReadUInt8() << 8) |
                ReadUInt8());
        }

        public void WriteUShort(ushort value)
        {
            Write(new[]
            {
                (byte) ((value & 0xFF00) >> 8),
                (byte) (value & 0xFF)
            }, 0, 2);
        }

        public short ReadShort()
        {
            return (short)ReadUShort();
        }

        public void WriteShort(short value)
        {
            WriteUShort((ushort)value);
        }

        public uint ReadUInt()
        {
            return (uint)(
                (ReadUInt8() << 24) |
                (ReadUInt8() << 16) |
                (ReadUInt8() << 8) |
                ReadUInt8());
        }

        public void WriteUInt(uint value)
        {
            Write(new[]
            {
                (byte) ((value & 0xFF000000) >> 24),
                (byte) ((value & 0xFF0000) >> 16),
                (byte) ((value & 0xFF00) >> 8),
                (byte) (value & 0xFF)
            }, 0, 4);
        }

        public int ReadInt()
        {
            return (int)ReadUInt();
        }

        public void WriteInt(int value)
        {
            WriteUInt((uint)value);
        }

        public ulong ReadULong()
        {
            return ((ulong)ReadUInt8() << 56) |
                   ((ulong)ReadUInt8() << 48) |
                   ((ulong)ReadUInt8() << 40) |
                   ((ulong)ReadUInt8() << 32) |
                   ((ulong)ReadUInt8() << 24) |
                   ((ulong)ReadUInt8() << 16) |
                   ((ulong)ReadUInt8() << 8) |
                   ReadUInt8();
        }

        public void WriteULong(ulong value)
        {
            Write(new[]
            {
                (byte) ((value & 0xFF00000000000000) >> 56),
                (byte) ((value & 0xFF000000000000) >> 48),
                (byte) ((value & 0xFF0000000000) >> 40),
                (byte) ((value & 0xFF00000000) >> 32),
                (byte) ((value & 0xFF000000) >> 24),
                (byte) ((value & 0xFF0000) >> 16),
                (byte) ((value & 0xFF00) >> 8),
                (byte) (value & 0xFF)
            }, 0, 8);
        }

        public long ReadLong()
        {
            return (long)ReadULong();
        }

        public void WriteLong(long value)
        {
            WriteULong((ulong)value);
        }

        public byte[] ReadByteArray(int length)
        {
            var result = new byte[length];
            if (length == 0) return result;
            Read(result, 0, result.Length);
            return result;
        }

        public void WriteByteArray(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        public void WriteByteArray(byte[] value, int offset, int count)
        {
            Write(value, offset, count);
        }

        public void WriteBytes(byte[] data)
        {
            WriteByteArray(data);
        }

        public sbyte[] ReadSByteArray(int length)
        {
            return (sbyte[])(Array)ReadByteArray(length);
        }

        public void WriteSByteArray(sbyte[] value)
        {
            Write((byte[])(Array)value, 0, value.Length);
        }

        public ushort[] ReadUShortArray(int length)
        {
            var result = new ushort[length];
            if (length == 0) return result;
            for (var i = 0; i < length; i++)
                result[i] = ReadUShort();
            return result;
        }

        public void WriteUShortArray(ushort[] value)
        {
            for (ushort i = 0; i < value.Length; i++)
                WriteUShort(i);
        }

        public short[] ReadShortArray(int length)
        {
            return (short[])(Array)ReadUShortArray(length);
        }

        public void WriteShortArray(short[] value)
        {
            WriteUShortArray((ushort[])(Array)value);
        }

        public uint[] ReadUIntArray(int length)
        {
            var result = new uint[length];
            if (length == 0) return result;
            for (var i = 0; i < length; i++)
                result[i] = ReadUInt();
            return result;
        }

        public void WriteUIntArray(uint[] value)
        {
            for (var index = 0; index < value.Length; index++)
                WriteUInt(value[index]);
        }

        public int[] ReadIntArray(int length)
        {
            return (int[])(Array)ReadUIntArray(length);
        }

        public void WriteIntArray(int[] value)
        {
            WriteUIntArray((uint[])(Array)value);
        }

        public ulong[] ReadULongArray(int length)
        {
            var result = new ulong[length];
            if (length == 0) return result;
            for (var i = 0; i < length; i++)
                result[i] = ReadULong();
            return result;
        }

        public void WriteULongArray(ulong[] value)
        {
            for (var i = 0; i < value.Length; i++)
                WriteULong(value[i]);
        }

        public long[] ReadLongArray(int length)
        {
            return (long[])(Array)ReadULongArray(length);
        }

        public void WriteLongArray(long[] value)
        {
            WriteULongArray((ulong[])(Array)value);
        }

        public unsafe float ReadFloat()
        {
            var value = ReadUInt();
            return *(float*)&value;
        }

        public unsafe void WriteFloat(float value)
        {
            WriteUInt(*(uint*)&value);
        }

        public unsafe double ReadDouble()
        {
            var value = ReadULong();
            return *(double*)&value;
        }

        public unsafe void WriteDouble(double value)
        {
            WriteULong(*(ulong*)&value);
        }

        public bool ReadBoolean()
        {
            return ReadUInt8() != 0;
        }

        public void WriteBoolean(bool value)
        {
            WriteUInt8(value ? (byte)1 : (byte)0);
        }

        public string ReadString()
        {
            long length = ReadVarInt();
            if (length == 0) return string.Empty;
            var data = ReadByteArray((int)length);
            return StringEncoding.GetString(data);
        }

        public void WriteString(string value)
        {
            WriteVarInt(StringEncoding.GetByteCount(value));
            if (value.Length > 0)
                WriteByteArray(StringEncoding.GetBytes(value));
        }

        public void WriteUUID(string value)
        {
            var g = new Guid(value).ToByteArray();
            var long1 = new byte[8];
            var long2 = new byte[8];
            Array.Copy(g, 0, long1, 0, 8);
            Array.Copy(g, 8, long2, 0, 8);
            WriteByteArray(long1);
            WriteByteArray(long2);
        }

        public string ReadUUID()
        {
            return ""; //TODO: Actually implement this? :D
        }

        public void WritePosition(Vector3 position)
        {
            var x = (long)position.X;
            var y = (long)position.Y;
            var z = (long)position.Z;
            WriteLong(((x & 0x3FFFFFF) << 38) | ((y & 0xFFF) << 26) | (z & 0x3FFFFFF));
        }

        public Vector3 ReadPosition()
        {
            var val = ReadLong();
            float x = val >> 38;
            float y = (val >> 26) & 0xFFF;
            float z = val << 38 >> 38;

            return new Vector3(x, y, z);
        }

        public void WritePacketState(PacketState state)
        {
            WriteVarInt((int)state);
        }

        public PacketState ReadPacketState()
        {
            return (PacketState)ReadVarInt();
        }

        public void WriteStatusResponse(StatusResponse response)
        {
            WriteString(JsonConvert.SerializeObject(response));
        }

        public StatusResponse ReadStatusResponse()
        {
            return JsonConvert.DeserializeObject<StatusResponse>(ReadString());
        }

        public byte[] ReadBytes()
        {
            int length = ReadVarInt();
            return ReadByteArray(length);
        }

        public void WriteNBTCompound(NbtCompound compound)
        {
            NbtFile file = new NbtFile(compound);
            file.SaveToStream(this, NbtCompression.None);
        }
    }
}
