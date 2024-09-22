using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PSO2SERVER.Packets;

namespace PSO2SERVER.Models
{
    public enum ObjectType : UInt16
    {
        Unknown = 0,
        Player = 4,
        Map = 5,
        Object = 6,
        StaticObject = 7,
        Quest = 11,
        Party = 13,
        World = 16,
        APC = 22,
        Undefined = 0xFFFF
    }

    public struct ObjectHeader
    {
        /// Id of the object.
        public UInt32 ID;
        public UInt32 padding; // Always is padding
        /// Type of the object.
        public ObjectType ObjectType;
        /// Zone id of the object. Not set for players.
        public UInt16 MapID;

        public ObjectHeader(uint id, ObjectType type) : this()
        {
            ID = id;
            ObjectType = type;
        }

        /// Zone id of the object. Not set for players.
        public ObjectHeader(uint id, ObjectType type, UInt16 mapid) : this()
        {
            ID = id;
            ObjectType = type;
            MapID = mapid;
        }
        public void ReadFromStream(PacketReader reader)
        {
            ID = reader.ReadUInt32();
            padding = reader.ReadUInt32(); // 读取填充
            ObjectType = (ObjectType)reader.ReadUInt16();
            MapID = reader.ReadUInt16();
        }

        public void WriteToStream(PacketWriter writer)
        {
            writer.Write(ID);
            writer.Write(padding); // 写入填充
            writer.Write((UInt16)ObjectType);
            writer.Write(MapID);
        }
    }
}
