using System;
using System.IO;
using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    class ServerHelloPacket : Packet
    {
        private readonly ushort _Unk1;
        private readonly ushort _BlockId;
        private readonly uint _Unk2;

        public ServerHelloPacket(ushort Unk1, ushort BlockId, uint Unk2)
        {
            _Unk1 = Unk1;
            _BlockId = BlockId;
            _Unk2 = Unk2;
        }

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write(_Unk1);
            writer.Seek(4, SeekOrigin.Current); // Skip 4 bytes
            writer.Write(_BlockId);
            writer.Write(_Unk2);
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x03, 0x08, PacketFlags.None);
        }
    }
}
