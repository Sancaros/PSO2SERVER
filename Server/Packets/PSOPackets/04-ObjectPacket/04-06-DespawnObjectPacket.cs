using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class DespawnObjectPacket : Packet
    {
        private readonly PSOObject _player;
        private readonly PSOObject _item;
        public DespawnObjectPacket(PSOObject player, PSOObject item)
        {
            _player = player;
            _item = item;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteStruct(_player.Header);
            writer.WriteStruct(_item.Header);
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x04, 0x06, PacketFlags.None);
        }

        #endregion
    }
}