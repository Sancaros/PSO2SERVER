using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PSO2SERVER.Packets
{
    public class PacketReader : BinaryReader
    {
        private byte[] _data;    // 存储数据的字节数组
        private int _position;    // 当前读取位置
        private int _size;        // 数据的总大小

        public int Position => _position;

        public PacketReader(Stream s) : base(s)
        {
        }

        public PacketReader(byte[] bytes) : base(new MemoryStream(bytes))
        {
        }

        public PacketReader(byte[] bytes, uint position, uint size)
            : base(new MemoryStream(bytes, (int) position, (int) size))
        {
            _data = bytes;
            _position = 0;
            _size = (int)(size);
        }

        public uint ReadMagic(uint xor, uint sub)
        {
            return (ReadUInt32() ^ xor) - sub;
        }

        // 示例方法：读取 ASCII 字符串
        public string ReadAsciiForPosition(int start, int length)
        {
            try
            {
                if (_position + length > _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(start), $"Reading beyond data boundary in {nameof(ReadAsciiForPosition)}");
                }

                var result = Encoding.ASCII.GetString(_data, _position, length);
                _position += length;
                return result;
            }
            catch (Exception ex)
            {
                Logger.WriteException(nameof(ReadAsciiForPosition), ex);
                throw;
            }
        }

        public string ReadAscii(uint xor, uint sub)
        {
            var magic = ReadMagic(xor, sub);

            if (magic == 0)
            {
                return string.Empty;
            }
            var charCount = magic - 1;
            var padding = 4 - (charCount & 3);
            //Logger.Write("charCount = " + charCount + " padding = " + padding);

            var data = ReadBytes((int) charCount);
            for (var i = 0; i < padding; i++)
                ReadByte();

            return Encoding.ASCII.GetString(data);
        }

        public string ReadFixedLengthAscii(uint charCount)
        {
            var data = ReadBytes((int) charCount);
            var str = Encoding.ASCII.GetString(data);

            var endAt = str.IndexOf('\0');
            if (endAt == -1)
                return str;
            return str.Substring(0, endAt);
        }

        public string ReadUtf16(uint xor, uint sub)
        {
            var magic = ReadMagic(xor, sub);

            if (magic == 0)
            {
                return "";
            }
            var charCount = magic - 1;
            var padding = (magic & 1);

            var data = ReadBytes((int) (charCount*2));
            ReadUInt16();
            if (padding != 0)
                ReadUInt16();

            return Encoding.GetEncoding("UTF-16").GetString(data);
        }

        public string ReadFixedLengthUtf16(int charCount)
        {
            var data = ReadBytes(charCount*2);
            var str = Encoding.GetEncoding("UTF-16").GetString(data);

            var endAt = str.IndexOf('\0');
            if (endAt == -1)
                return str;
            return str.Substring(0, endAt);
        }

        public T ReadStruct<T>() where T : struct
        {
            var structBytes = new byte[Marshal.SizeOf(typeof (T))];
            Read(structBytes, 0, structBytes.Length);

            return Helper.ByteArrayToStructure<T>(structBytes);
        }

        public List<T> ReadList<T>() where T : struct
        {
            var count = ReadUInt32();
            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                list.Add(ReadStruct<T>());
            }
            return list;
        }
        public string ReadNullableString(uint xor, uint sub)
        {
            var magic = ReadMagic(xor, sub);
            if (magic == 0)
            {
                return null;
            }

            var charCount = magic - 1;
            var data = ReadBytes((int)charCount);
            return data?.Length > 0 ? Encoding.ASCII.GetString(data) : null;
        }

        public PSOLocation ReadEntityPosition()
        {
            PSOLocation pos = new PSOLocation()
            {
                RotX = Helper.FloatFromHalfPrecision(ReadUInt16()),
                RotY = Helper.FloatFromHalfPrecision(ReadUInt16()),
                RotZ = Helper.FloatFromHalfPrecision(ReadUInt16()),
                RotW = Helper.FloatFromHalfPrecision(ReadUInt16()),
                PosX = Helper.FloatFromHalfPrecision(ReadUInt16()),
                PosY = Helper.FloatFromHalfPrecision(ReadUInt16()),
                PosZ = Helper.FloatFromHalfPrecision(ReadUInt16()),
            };

            return pos;
        }
    }
}