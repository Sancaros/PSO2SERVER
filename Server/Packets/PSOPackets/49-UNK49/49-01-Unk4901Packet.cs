using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class Unk4901Packet : Packet
    {

        public Unk4901Packet()
        {
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            byte[] data = new byte[]
            {
            0x78, 0x00, 0x78, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xF4, 0x01, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xAC, 0x0D, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
            0xB8, 0x0B, 0x00, 0x00,
            };
            return data;
            //var pkt = new PacketWriter();
            //pkt.Write((byte)0x78);
            //pkt.Write((byte)0);
            //pkt.Write((byte)0x78);
            //pkt.Write((byte)0);
            //pkt.Write((byte)0);
            //pkt.Write((byte)0);
            //pkt.Write((byte)0x04);
            //pkt.Write((byte)0);
            //pkt.Write((byte)0x64);
            //return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x49, 0x01, PacketFlags.None);
        }

        #endregion
    }
}