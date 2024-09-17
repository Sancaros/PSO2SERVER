using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class EnemySpawnPacket : Packet
    {

        public EnemySpawnPacket()
        {
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x08, 0x0D, PacketFlags.None);
        }

        #endregion
    }
}