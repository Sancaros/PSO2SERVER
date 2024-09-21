using Mysqlx.Crud;
using Mysqlx.Session;
using Org.BouncyCastle.Utilities;
using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class MovementActionServerPacket : Packet
    {
        private readonly int _user_playerid;
        private readonly ObjectHeader _preformer;
        private readonly byte[] _preData;
        private readonly string _command;
        private readonly byte[] _rest;
        private readonly uint _thingCount;
        private readonly byte[] _things;
        private readonly byte[] _final;
        public MovementActionServerPacket(int user_playerid, ObjectHeader preformer, byte[] preData
            , string command, byte[] rest, uint thingCount, byte[] things, byte[] final
            )
        {
            _user_playerid = user_playerid;
            _preformer = preformer;
            _preData = preData;
            _command = command;
            _rest = rest;
            _thingCount = thingCount;
            _things = things;
            _final = things;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            PacketWriter output = new PacketWriter();
            output.WriteStruct(new ObjectHeader((uint)_user_playerid, ObjectType.Player));
            output.WriteStruct(_preformer);
            output.Write(_preData);
            output.WriteAscii(_command, 0x4315, 0x7A);
            output.Write(_rest);
            output.WriteMagic(_thingCount, 0x4315, 0x7A);
            output.Write(_things);
            output.Write(_final);

            return output.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x04, 0x80, PacketFlags.unk0x44);
        }

        #endregion
    }
}