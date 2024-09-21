using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class Unk4A03Packet : Packet
    {
        public struct Unk4A03Packet_t
        {
            public uint Unk1; // 对应 Rust 的 u32
            public List<Mission> Unk2; // 使用 List 替代 Vec
            public List<uint> Unk3; // 使用 List 替代 Vec
            public List<Unk2Struct> Unk4; // 使用 List 替代 Vec
            public uint Unk5; // 对应 Rust 的 u32
        }

        Unk4A03Packet_t pkt = new Unk4A03Packet_t();

        public Unk4A03Packet(Mission mission, uint unk3, Unk2Struct unk4)
        {
            pkt.Unk2.Add(mission);
            pkt.Unk3.Add(unk3);
            pkt.Unk4.Add(unk4);
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var writer = new PacketWriter();
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x4A, 0x03, PacketFlags.None);
        }

        #endregion
    }
}