using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class ClientPongPacket : Packet
    {
        private readonly ulong _clientTime;
        public ClientPongPacket(ulong clientTime)
        {
            _clientTime = clientTime;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.Write(_clientTime);
            pkt.Write(Helper.Timestamp(DateTime.UtcNow));
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0x0E, PacketFlags.None);
        }

        #endregion
    }
}