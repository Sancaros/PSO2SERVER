using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class DespawnObjectPacket : Packet
    {
        private readonly byte _subtype;
        private readonly byte _type;

        public DespawnObjectPacket()
        {
            _type = 0x04;
            _subtype = 0x06;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            return new byte[0];
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader
            {
                Type = _type,
                Subtype = _subtype
            };
        }

        #endregion
    }
}