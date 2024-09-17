using Org.BouncyCastle.Utilities;
using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using static PSO2SERVER.Models.PSOObject;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class NPCSpawnPacket : Packet
    {
        private readonly PSOObject _obj;
        public NPCSpawnPacket(PSOObject obj)
        {
            _obj = obj;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteStruct(_obj.Header);
            writer.Write(_obj.Position);
            writer.Seek(2, SeekOrigin.Current); // Padding I guess...
            writer.WriteFixedLengthASCII(_obj.Name, 0x20);

            writer.Write(0); // Padding?
            writer.Write(new byte[0xC]); // Unknown, usually zero

            writer.Write((UInt16)0);
            writer.Write((UInt16)0);

            writer.Write((UInt32)0);
            writer.Write((UInt32)0);

            writer.Write((UInt32)1101004800); // Always this

            writer.Write((UInt32)0);
            writer.Write((UInt32)0);
            writer.Write((UInt32)0);

            writer.Write((UInt32)1);

            writer.WriteMagic(1, 0x9FCD, 0xE7);
            writer.Write((UInt32)0);

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x08, 0x0C, PacketFlags.None);
        }

        #endregion
    }
}