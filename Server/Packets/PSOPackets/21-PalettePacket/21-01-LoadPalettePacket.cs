﻿using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class LoadPalettePacket : Packet
    {
        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var writer = new PacketWriter();

            // Enable flag
            writer.Write((byte) 1);

            // Blank out the rest (skills)
            for (var i = 0; i < 1091; i++)
                writer.Write((byte) 0);

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x21, 0x01, PacketFlags.None);
        }

        #endregion
    }
}