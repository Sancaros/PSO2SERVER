using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.Handlers;

namespace PSO2SERVER.Packets.PSOPackets
{
    class MovementPacket : Packet
    {
        public struct PackedVec4
        {
            public UInt16 x, y, z, w;

            public PackedVec4(PSOLocation location)
            {
                this.x = Helper.FloatToHalfPrecision(location.RotX);
                this.y = Helper.FloatToHalfPrecision(location.RotY);
                this.z = Helper.FloatToHalfPrecision(location.RotZ);
                this.w = Helper.FloatToHalfPrecision(location.RotW);
            }
        }

        public struct PackedVec3
        {
            public UInt16 x, y, z;

            public PackedVec3(PSOLocation location)
            {
                this.x = Helper.FloatToHalfPrecision(location.PosX);
                this.y = Helper.FloatToHalfPrecision(location.PosY);
                this.z = Helper.FloatToHalfPrecision(location.PosZ);
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x38)]
        public struct FullMovementData
        {
            [FieldOffset(0x0)]
            public ObjectHeader entity1;
            [FieldOffset(0xC)]
            public ObjectHeader entity2;
            [FieldOffset(0x18)]
            public UInt32 timestamp;
            [FieldOffset(0x1C)]
            public PackedVec4 rotation;
            [FieldOffset(0x24)]
            public PackedVec3 currentPos;
            [FieldOffset(0x2A)]
            public UInt16 Unknown2; // This MAY be part of lastPos, as lastPos may be a Vec4?
            [FieldOffset(0x2C)]
            public PackedVec3 unknownPos;
            [FieldOffset(0x32)]
            public UInt16 Unknown3; // This MAY be part of currentPos, as lastPos may be a Vec4?
            [FieldOffset(0x34)]
            public UInt32 Unknown4;
        }

        FullMovementData data;

        public MovementPacket(FullMovementData data)
        {
            this.data = data;
        }

        public override byte[] Build()
        {
            PacketWriter pw = new PacketWriter();
            pw.WriteStruct(data);
            return pw.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x04, 0x07, PacketFlags.OBJECT_RELATED);
        }
    }
}
