using System;
using System.Runtime.InteropServices;

namespace PSO2SERVER.Models
{
    public struct PacketHeader
    {
        public UInt32 Size;
        public byte Type;
        public byte Subtype;
        public byte Flags1;
        public byte Flags2;

        public PacketHeader(int size, byte type, byte subtype, byte flags1, byte flags2)
        {
            this.Size = (uint)size;
            this.Type = type;
            this.Subtype = subtype;
            this.Flags1 = flags1;
            this.Flags2 = flags2;
        }

        public PacketHeader(byte type, byte subtype) : this(type, subtype, (byte)0)
        {
        }

        public PacketHeader(byte type, byte subtype, byte flags1) : this(0, type, subtype, flags1, 0)
        {
        }

        public PacketHeader(byte type, byte subtype, PacketFlags packetFlags) : this(type, subtype, (byte)packetFlags)
        {
        }
    }

    /// Packet flags.
    [Flags]
    public enum PacketFlags : byte
    {
        /// 0x00
        None = 0x00,
        /// Set when the packet contains variable length data. 0x04
        PACKED = 1 << 2,
        /// 0x10
        FLAG_10 = 1 << 4,
        /// Set when the [`Packet::Movement`] has all fields set. 0x20
        FULL_MOVEMENT = 1 << 5,
        /// Set for all (?) of (0x04) packets. 0x40
        OBJECT_RELATED = 1 << 6,
        /// 0x44
        unk0x44 = 0x44
    }
}

