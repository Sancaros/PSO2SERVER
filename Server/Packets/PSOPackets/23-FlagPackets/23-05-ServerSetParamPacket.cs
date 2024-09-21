using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PSO2SERVER.Models.Flags;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class ServerSetParamPacket : Packet
    {
        /// Flag type.
        public FlagType _param_type { get; set; }
        /// Flag ID.
        public uint _id { get; set; }
        /// Flag value.
        public uint _value { get; set; }

        public ServerSetParamPacket(uint flag, uint value)
        {
            _param_type = FlagType.Account;
            _id = flag;
            _value = value;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.Write((uint)_param_type);
            pkt.Write((uint)_id);
            pkt.Write((uint)_value);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x23, 0x05, PacketFlags.None);
        }

        #endregion
    }
}