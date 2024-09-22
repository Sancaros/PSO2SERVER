using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class NewDefaultPAsPacket : Packet
    {

        public const int FixedLength = 0x1A0; // 416 bytes
        public const int SeekAfter = 0x240; // 576 bytes

        public List<uint> Default { get; set; }

        public NewDefaultPAsPacket()
        {
            Default = new List<uint>(FixedLength / sizeof(uint)); // 初始化为 0x1A0 / 4
        }

        public void ReadFromStream(PacketReader reader)
        {
            // 读取 Default
            for (int i = 0; i < Default.Capacity; i++)
            {
                Default.Add(reader.ReadUInt32());
            }

            // 跳过填充
            reader.BaseStream.Seek(SeekAfter - FixedLength, SeekOrigin.Current);
        }

        public void WriteToStream(PacketWriter writer)
        {
            // 写入 Default
            foreach (var value in Default)
            {
                writer.Write(value);
            }

            // 填充到 0x240
            long paddingSize = SeekAfter - FixedLength;
            writer.BaseStream.Seek(paddingSize, SeekOrigin.Current);
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var writer = new PacketWriter();
            foreach (var value in Default)
            {
                writer.Write(value);
            }
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x21, 0x0F, PacketFlags.None);
        }

        #endregion
    }
}