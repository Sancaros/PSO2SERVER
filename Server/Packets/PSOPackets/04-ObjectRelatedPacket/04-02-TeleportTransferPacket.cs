﻿using System;
using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    class TeleportTransferPacket : Packet
    {
        /// (0x04, 0x02) Object Teleport Location.
        private PSOObject src;
        private PSOLocation dst;

        public TeleportTransferPacket(PSOObject srcTeleporter, PSOLocation destination)
        {
            src = srcTeleporter;
            dst = destination;
        }

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write(new byte[12]);
            writer.WriteStruct(src.Header);
            writer.WritePosition(dst);
            writer.Write(new byte[2]);
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x04, 0x02, PacketFlags.OBJECT_RELATED);
        }
    }
}
