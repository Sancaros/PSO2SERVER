using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class SetPlayerIDPacket : Packet
    {
        private readonly int _PlayerId;

        public SetPlayerIDPacket(int PlayerId)
        {
            _PlayerId = PlayerId;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.WritePlayerHeader((uint)_PlayerId);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x06, 0x00, PacketFlags.None);
        }

        #endregion
    }
}