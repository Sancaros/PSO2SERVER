using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PSO2SERVER.Models.Flags;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class ServerSetFlagPacket : Packet
    {

        /// Flag type.
        public FlagType _flag_type { get; set; }
        /// Flag ID.
        public uint _id { get; set; }
        /// Flag value.
        public uint _value { get; set; }
        public uint _unk { get; set; }

        public ServerSetFlagPacket(uint flag, uint value)
        {
            _flag_type = FlagType.Account;
            _id = flag;
            _value = value;
            _unk = 0; // 设定未知字段的默认值
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.Write((uint)_flag_type);
            pkt.Write((uint)_id);
            pkt.Write((uint)_value);
            pkt.Write((uint)_unk);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x23, 0x04, PacketFlags.None);
        }

        #endregion
    }
}