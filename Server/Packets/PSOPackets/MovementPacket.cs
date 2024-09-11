using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.Handlers;

namespace PSO2SERVER.Packets.PSOPackets
{
    class MovementPacket : Packet
    {
        FullMovementData data;

        public MovementPacket(FullMovementData data)
        {
            this.data = data;
        }

        public override byte[] Build()
        {
            PacketWriter pw = new PacketWriter();
            pw.WriteStruct(data);
            return pw.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x4, 0x7, PacketFlags.OBJECT_RELATED);
        }
    }
}
