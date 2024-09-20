using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class CharacterMovePacket : Packet
    {
        public struct CharacterShipTransferRightsPacket
        {
            public uint Status { get; set; }
            public uint AcPrice { get; set; }
            public uint Unk1 { get; set; }
            public uint Unk2 { get; set; }
            public uint Unk3 { get; set; }
            public uint Unk4 { get; set; }
            public uint Unk5 { get; set; }
        }

        private uint _account_id;
        private int _character_id;
        private CharacterShipTransferRightsPacket pkt = new CharacterShipTransferRightsPacket();

        public CharacterMovePacket(int character_id, uint account_id)
        {
            _account_id = account_id;
            _character_id = character_id;
            pkt = new CharacterShipTransferRightsPacket
            {
                Status = 0,
                AcPrice = 100
                // 其他字段初始化
            };
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var write = new PacketWriter();
            write.WriteStruct(pkt);
            return write.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0xB9, PacketFlags.None);
        }

        #endregion
    }
}