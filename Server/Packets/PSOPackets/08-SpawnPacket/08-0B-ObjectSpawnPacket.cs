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
    public class ObjectSpawnPacket : Packet
    {
        private readonly PSOObject _obj;
        public ObjectSpawnPacket(PSOObject obj)
        {
            _obj = obj;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteStruct(_obj.Header);
            writer.WritePosition(_obj.Position);
            writer.Seek(2, SeekOrigin.Current); // Padding I guess...
            writer.WriteFixedLengthASCII(_obj.Name, 0x34);
            writer.Write(_obj.ThingFlag);
            writer.Write(_obj.Things.Length);
            foreach (PSOObjectThing thing in _obj.Things)
            {
                writer.WriteStruct(thing);
            }

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x08, 0x0B, PacketFlags.None);
        }

        #endregion
    }
}