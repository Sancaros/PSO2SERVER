using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class NicknameRequestPacket : Packet
    {
        // Error flag
        //[DataMember(Order = 1)]
        public ushort Error { get; set; }

        public NicknameRequestPacket()
        {
            Error = 0; // 默认值
        }

        public NicknameRequestPacket(ushort error)
        {
            Error = error;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.Write(new byte[0x42]);
            pkt.Write(Error);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0x1E, PacketFlags.None);
        }

        #endregion
    }
}