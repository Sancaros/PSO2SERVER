using Mysqlx.Crud;
using Mysqlx.Session;
using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class ActionUpdateServerPacket : Packet
    {
        private readonly int _user_playerid;
        private readonly ObjectHeader _actor;
        private readonly byte[] _rest;
        public ActionUpdateServerPacket(int user_playerid, ObjectHeader actor, byte[] rest)
        {
            _user_playerid = user_playerid;
            _actor = actor;
            _rest = rest;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteStruct(new ObjectHeader((uint)_user_playerid, EntityType.Player));
            writer.WriteStruct(_actor);
            writer.Write(_rest);

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x04, 0x81, PacketFlags.OBJECT_RELATED);
        }

        #endregion
    }
}