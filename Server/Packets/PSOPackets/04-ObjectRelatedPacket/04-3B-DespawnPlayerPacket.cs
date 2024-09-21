using Mysqlx.Crud;
using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class DespawnPlayerPacket : Packet
    {
        private readonly int _other_playerid;
        private readonly int _user_playerid;
        public DespawnPlayerPacket(int other_playerid, int user_playerid)
        {
            _other_playerid = other_playerid;
            _user_playerid = user_playerid;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteStruct(new ObjectHeader((uint)_other_playerid, ObjectType.Player));
            writer.WriteStruct(new ObjectHeader((uint)_user_playerid, ObjectType.Player));
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x04, 0x3B, PacketFlags.OBJECT_RELATED);
        }

        #endregion
    }
}