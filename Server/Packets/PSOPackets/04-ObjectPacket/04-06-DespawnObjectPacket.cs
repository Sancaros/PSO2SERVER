﻿using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class DespawnObjectPacket : Packet
    {
        public DespawnObjectPacket()
        {
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            return new byte[0];
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x04, 0x06, PacketFlags.None);
        }

        #endregion
    }
}