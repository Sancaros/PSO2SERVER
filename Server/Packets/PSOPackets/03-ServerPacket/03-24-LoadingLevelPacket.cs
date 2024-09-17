using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class LoadingLevelPacket : Packet
    {
        private readonly string _file;

        public LoadingLevelPacket(string file)
        {
            _file = file;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var setAreaPacket = File.ReadAllBytes("Resources\\quests\\" + _file + ".bin");
            return setAreaPacket;
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x03, 0x24, PacketFlags.PACKED);
        }

        #endregion
    }
}