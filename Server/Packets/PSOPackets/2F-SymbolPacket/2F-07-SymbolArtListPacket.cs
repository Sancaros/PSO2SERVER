﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    class SymbolArtListPacket : Packet
    {
        ObjectHeader player;
        public SymbolArtListPacket(ObjectHeader player)
        {
            this.player = player;
        }

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteStruct(player);
            writer.Write(-1); // Dunno what goes here
            writer.WriteMagic(0x20, 0xE80C, 0xED); // 20 Things
            writer.Write(new Byte[16 * 0x20]); // 16 Bytes each

            return writer.ToArray();

        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x2F, 0x07, PacketFlags.PACKED);
        }
    }
}
