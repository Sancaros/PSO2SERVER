using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class NewDefaultPAsPacket : Packet
    {
        public uint[] Default { get; set; } = new uint[0x1A0]; // 默认大小为0x1A0

        public NewDefaultPAsPacket()
        {
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