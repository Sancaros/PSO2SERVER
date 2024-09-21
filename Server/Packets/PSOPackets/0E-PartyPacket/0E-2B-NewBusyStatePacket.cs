using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public enum BusyState
    {
        NotBusy,
        Busy,
    }

    public class NewBusyStatePacket : Packet
    {
        private int _user_playerid;
        private BusyState _state;

        public NewBusyStatePacket(int user_playerid, BusyState state)
        {
            _user_playerid = user_playerid;
            _state = state;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.WriteStruct(new ObjectHeader((uint)_user_playerid, ObjectType.Player));
            pkt.Write((uint)_state);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x0E, 0x2B, PacketFlags.None);
        }

        #endregion
    }
}