using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class ChatStatusPacket : Packet
    {
        /// Object of the player (not set for C -> S).
        public ObjectHeader Obj { get; set; }
        /// Chat status.
        public UInt32 Status { get; set; }

        public ChatStatusPacket(UInt32 sender_id, UInt32 status)
        {
            Obj = new ObjectHeader() { ID = sender_id, ObjectType = ObjectType.Player };
            Status = status;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.WriteStruct(Obj);
            pkt.Write(Status);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x0E, 0x19, PacketFlags.OBJECT_RELATED);
        }

        #endregion
    }
}