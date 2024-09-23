using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class RemoveMemberPacket : Packet
    {
        public ObjectHeader removed_member;
        public ObjectHeader receiver;

        public RemoveMemberPacket(uint removed_member_id, uint receiver_id)
        {
            removed_member.ID = removed_member_id;
            removed_member.ObjectType = ObjectType.Player;
            receiver.ID = receiver_id;
            receiver.ObjectType = ObjectType.Player;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.WriteStruct(removed_member);
            pkt.WriteStruct(receiver);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x0E, 0x01, PacketFlags.None);
        }

        #endregion
    }
}