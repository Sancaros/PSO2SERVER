using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class AccountFlagsPacket : Packet
    {
        /// <summary>
        /// Account flags.
        /// </summary>
        public List<byte> Flags { get; set; } = new List<byte>(0x400);

        /// <summary>
        /// Account parameters.
        /// </summary>
        public List<uint> Params { get; set; } = new List<uint>(0x100);

        public AccountFlagsPacket()
        {
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x23, 0x06, PacketFlags.None);
        }

        #endregion
    }
}