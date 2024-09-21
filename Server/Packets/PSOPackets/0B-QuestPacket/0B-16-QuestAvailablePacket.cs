using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSO2SERVER.Models;
using System.Runtime.InteropServices;

namespace PSO2SERVER.Packets.PSOPackets
{
    class QuestAvailablePacket : Packet
    {
        public short[] amount = new short[Enum.GetValues(typeof(QuestType)).Length];
        QuestTypeAvailable available = QuestTypeAvailable.Arks;

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();

            // Filler/Padding?
            writer.Write((UInt16)0);

            // Amounts
            for (int i = 0; i < amount.Length; i++)
            {
                amount[i] = 1; // Just for testing
                writer.Write(amount[i]);
            }

            // Padding/Blank entries?
            for (int i = 0; i < 2; i++)
                writer.Write((int)0);

            // Available Bitfield
            writer.Write((UInt64)available);

            // Filler/Padding?
            for (int i = 0; i < 2; i++)
                writer.Write((int)0);

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x0B, 0x16);
        }
    }
}
