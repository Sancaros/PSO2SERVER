using System;
using System.IO;
using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    class ServerPingPacket : Packet
    {
        public ServerPingPacket()
        {
        }

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x03, 0x0B, PacketFlags.None);
        }
    }
}
