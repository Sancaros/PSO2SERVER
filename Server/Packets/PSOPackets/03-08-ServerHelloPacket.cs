using System;
using System.IO;
using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    class ServerHelloPacket : Packet
    {

        /// <summary>
        /// Unknown. Seems to be always 0x03.
        /// </summary>
        public ushort Unk1 { get; set; }

        /// <summary>
        /// Block Id.
        /// </summary>
        public ushort BlockId { get; set; }

        public uint Unk2 { get; set; }

        public ServerHelloPacket()
        {
            Unk1 = 0x03; // Initialize Unk1 with default value
        }

        public override byte[] Build()
        {

            //var welcome = new PacketWriter();
            //welcome.Write((ushort)3);
            //welcome.Write((ushort)201);
            //welcome.Write((ushort)0);
            //welcome.Write((ushort)0);

            PacketWriter writer = new PacketWriter();
            writer.Write(Unk1);
            writer.Seek(4, SeekOrigin.Current); // Skip 4 bytes
            writer.Write(BlockId);
            writer.Write(Unk2);
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x03, 0x08, PacketFlags.None);
        }
    }
}
