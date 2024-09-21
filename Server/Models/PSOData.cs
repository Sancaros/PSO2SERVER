using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
