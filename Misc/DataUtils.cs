using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCSharp.Misc.Data;

namespace MCSharp.Misc
{
    static class DataUtils
    {
        public static void WriteVarInt(this Stream stream, int Value)
        {
            do
            {
                byte temp = (byte)(Value & 127);
                Value = (int)((uint)Value >> 7);
                if (Value != 0)
                {
                    temp |= 128;
                }
                stream.WriteByte(temp);
            } while (Value != 0);
        }
        public static int MeasureVarInt(int Value)
        {
            int len = 0;
            do
            {
                byte temp = (byte)(Value & 127);
                Value = (int)((uint)Value >> 7);
                if (Value != 0)
                {
                    temp |= 128;
                }
                //stream.WriteByte(temp);
                len++;
            } while (Value != 0);
            return len;
        }
        public static void WriteVarLong(this Stream stream, long Value)
        {
            do
            {
                byte temp = (byte)(Value & 127);
                Value = (int)((uint)Value >> 7);
                if (Value != 0)
                {
                    temp |= 128;
                }
                stream.WriteByte(temp);
            } while (Value != 0);
        }
        public static int ReadVarInt(this Stream stream)
        {
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                read = (byte)stream.ReadByte();
                int value = (read & 127);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 5)
                {
                    throw new ArgumentException("VarInt is too big");
                }
            } while ((read & 128) != 0);
            return result;
        }
        public static int ReadVarInt(this Stream stream, out int Read)
        {
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                read = (byte)stream.ReadByte();
                int value = (read & 127);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 5)
                {
                    throw new ArgumentException("VarInt is too big");
                }
            } while ((read & 128) != 0);
            Read = numRead;
            return result;
        }
        public static int ReadVarInt(this Stream stream, byte InitialData)
        {
            int numRead = 0;
            int result = 0;
            byte read = InitialData;
            do
            {
                if (numRead > 0)
                {
                    read = (byte)stream.ReadByte();
                }
                int value = (read & 127);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 5)
                {
                    throw new ArgumentException("VarInt is too big");
                }
            } while ((read & 128) != 0);
            return result;
        }
        public static long ReadVarLong(this Stream stream)
        {
            int numRead = 0;
            long result = 0;
            byte read;
            do
            {
                read = (byte)stream.ReadByte();
                int value = (read & 127);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 10)
                {
                    throw new ArgumentException("VarLong is too big");
                }
            } while ((read & 128) != 0);
            return result;
        }
        public static void WritePosition(this Stream stream, Vec3 Position)
        {
            ulong ander = 0xFFFFFFFFFFFFFFFF;
            ulong position = (uint)Position.X;
            position <<= 26;
            ander <<= 26;
            position |= ((uint)Position.Z & ~ander);
            position <<= 12;
            ander <<= 12;
            position |= ((uint)Position.Y & ~ander);
            stream.WriteNum(position);
        }
        public static void WriteLegacyPosition(this Stream stream, Vec3 Position)   // The "position" data type changed in 1.14. This is for encoding a "position" for a client older than 1.14 (to be exact, version 18w43a or protocol version 440 and above use the new position type)
        {
            ulong ander = 0xFFFFFFFFFFFFFFFF;
            ulong position = (uint)Position.X;
            position <<= 12;
            ander <<= 12;
            position |= ((uint)Position.Y & ~ander);
            position <<= 26;
            ander <<= 26;
            position |= ((uint)Position.Z & ~ander);
            stream.WriteNum(position);
        }
        public static void WriteNum(this Stream stream, long Value)
        {
            stream.Write(BitConverter.GetBytes(Value).Reverse().ToArray(), 0, 8);
        }
        public static long ReadLong(this Stream stream)
        {
            byte[] buf = new byte[8];
            stream.Read(buf, 0, 8);
            return BitConverter.ToInt64(buf.Reverse().ToArray(), 0);
        }
        public static void WriteNum(this Stream stream, ulong Value)
        {
            stream.Write(BitConverter.GetBytes(Value).Reverse().ToArray(), 0, 8);
        }
        public static ulong ReadUlong(this Stream stream)
        {
            byte[] buf = new byte[8];
            stream.Read(buf, 0, 8);
            return BitConverter.ToUInt64(buf.Reverse().ToArray(), 0);
        }
        public static void WriteNum(this Stream stream, int Value)
        {
            stream.Write(BitConverter.GetBytes(Value).Reverse().ToArray(), 0, 8);
        }
        public static int ReadInt(this Stream stream)
        {
            byte[] buf = new byte[4];
            stream.Read(buf, 0, 4);
            return BitConverter.ToInt32(buf.Reverse().ToArray(), 0);
        }
        public static void WriteNum(this Stream stream, uint Value)
        {
            stream.Write(BitConverter.GetBytes(Value).Reverse().ToArray(), 0, 8);
        }
        public static uint ReadUint(this Stream stream)
        {
            byte[] buf = new byte[4];
            stream.Read(buf, 0, 4);
            return BitConverter.ToUInt32(buf.Reverse().ToArray(), 0);
        }
        public static void WriteNum(this Stream stream, short Value)
        {
            stream.Write(BitConverter.GetBytes(Value).Reverse().ToArray(), 0, 8);
        }
        public static short ReadShort(this Stream stream)
        {
            byte[] buf = new byte[2];
            stream.Read(buf, 0, 2);
            return BitConverter.ToInt16(buf.Reverse().ToArray(), 0);
        }
        public static void WriteNum(this Stream stream, ushort Value)
        {
            stream.Write(BitConverter.GetBytes(Value).Reverse().ToArray(), 0, 8);
        }
        public static ushort ReadUshort(this Stream stream)
        {
            byte[] buf = new byte[2];
            stream.Read(buf, 0, 2);
            return BitConverter.ToUInt16(buf.Reverse().ToArray(), 0);
        }
        public static string ReadString(this Stream stream)
        {
            int len = stream.ReadVarInt();
            byte[] strUTF8 = new byte[len];
            stream.Read(strUTF8, 0, len);
            return Encoding.UTF8.GetString(strUTF8);
        }
        public static int MeasureString(string Value)
        {
            int length = Encoding.UTF8.GetByteCount(Value);
            length += MeasureVarInt(length);
            return length;
        }
        public static void WriteString(this Stream stream, string Value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(Value);
            stream.WriteVarInt(stringBytes.Length);
            stream.Write(stringBytes, 0, stringBytes.Length);
        }
        public static void WritePacketHeader(this Stream stream, int BodyLength, int PacketID)
        {
            int Length = BodyLength + MeasureVarInt(PacketID);
            stream.WriteVarInt(Length);
            stream.WriteVarInt(PacketID);
        }
    }
}
