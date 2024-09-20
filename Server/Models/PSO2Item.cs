using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static PSO2SERVER.Models.Character;

namespace PSO2SERVER.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ShortItemId
    {
        byte ItemType;
        byte Id;
        ushort Subid;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ItemId
    {
        ushort ItemType;
        ushort Id;
        ushort Unk3;
        ushort Subid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PSO2Items
    {
        long guid;
        ItemId id;
        Items data;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Items
    {
        [FieldOffset(0)]
        public PSO2ItemWeapon Weapon;
        [FieldOffset(0)]
        public PSO2ItemClothing Clothing;
        [FieldOffset(0)]
        public PSO2ItemConsumable Consumable;
        [FieldOffset(0)]
        public PSO2ItemCamo Camo;
        [FieldOffset(0)]
        public PSO2ItemUnit Unit;
        //[FieldOffset(0)]
        //public byte[] Unknown;
        //[FieldOffset(0)]
        //public PSO2ItemNone None;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PSO2ItemNone
    {
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PSO2ItemWeapon
    {
        byte flags;
        byte element;
        byte force;
        byte grind;
        byte grindPercent;
        byte unknown1;
        short unknown2;
        fixed short affixes[8];
        uint potential;
        byte extend;
        byte unknown3;
        ushort unknown4;
        uint unknown5;
        uint unknown6;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PSO2ItemClothing
    {
        ushort flags;
        fixed byte unk1[0x14];
        public HSVColor Color;
        fixed byte unk2[0xA];
        ushort Unk3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PSO2ItemConsumable
    {
        ushort flags;
        fixed byte unk1[0x24];
        ushort amount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PSO2ItemCamo
    {
        byte unk1;
        byte unk2;
        byte unk3;
        fixed byte unk4[0x24];
        byte unk5;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PSO2ItemUnit
    {
        byte flags;
        byte EnhLevel;
        byte EnhPercent;
        byte Unk1;

        // 使用 fixed 数组来存储附加信息
        fixed ushort Affixes[8]; // Item affix IDs (0 to 4095)

        fixed byte unk4[0x7];
        uint Potential;

        // 使用 fixed 数组来存储未知字段
        fixed byte Unk2[4];

        uint Unk3;
        ushort Unk4;
        ushort Unk5;

        // 提供访问固定数组的属性
        Span<ushort> AffixSpan
        {
            get
            {
                fixed (ushort* p = Affixes)
                {
                    return new Span<ushort>(p, 8);
                }
            }
        }
    }

    public enum ItemTypes
    {
        NoItem,
        Weapon,
        Clothing,
        Consumable,
        Camo,
        Unit,
        Unknown
    }

    [Flags]
    public enum ItemFlags
    {
        Locked = 0x01,
        BoundToOwner = 0x02
    }

    public enum ItemElement
    {
        None,
        Fire,
        Ice,
        Lightning,
        Wind,
        Light,
        Dark
    }

    public class PSO2Item
    {
        public const int Size = 0x38;

        MemoryStream stream;
        //TODO
        ItemTypes type = ItemTypes.Consumable;
        byte[] data = new byte[Size];

        public override string ToString()
        {
            return string.Format("Data: {0:X}", BitConverter.ToString(data)).Replace('-', ' ');
        }

        public PSO2Item(byte[] data)
        {
            SetData(data);
        }

        public byte[] GetData()
        {
            return data;
        }

        public void SetData(byte[] data)
        {
            this.data = data;

            stream = new MemoryStream(data, true);
        }

        public long GetGUID()
        {
            byte[] guid = new byte[sizeof(long)];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(guid, 0, sizeof(long));

            return BitConverter.ToInt64(guid, 0);
        }

        public void SetGUID(long guid)
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(guid), 0, 8);
        }

        public int[] GetID()
        {
            byte[] ID = new byte[sizeof(int)];
            byte[] subID = new byte[sizeof(int)];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(ID, 0x08, sizeof(int));
            stream.Read(subID, 0x0C, sizeof(int));

            return new int[] { BitConverter.ToInt32(ID, 0), BitConverter.ToInt32(subID, 0) };
        }

        public void SetID(int ID, int subID)
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(ID), 0x08, sizeof(int));
            stream.Write(BitConverter.GetBytes(subID), 0x0C, sizeof(int));
        }

        // ...
    }
}
public static class AffixUtils
{
    public static ushort[] ReadPackedAffixes(Stream reader)
    {
        byte[] packed = new byte[12];
        reader.Read(packed, 0, packed.Length);

        ushort[] affixes = new ushort[8];
        for (int i = 0; i < 4; i++)
        {
            affixes[i * 2] = BitConverter.ToUInt16(new byte[] { packed[i * 3], (byte)((packed[i * 3 + 2] & 0xF0) >> 4) }, 0);
            affixes[i * 2 + 1] = BitConverter.ToUInt16(new byte[] { packed[i * 3 + 1], (byte)(packed[i * 3 + 2] & 0xF) }, 0);
        }
        return affixes;
    }

    public static void WritePackedAffixes(ushort[] affixes, Stream writer)
    {
        byte[] packed = new byte[12];
        for (int i = 0; i < 4; i++)
        {
            byte[] affix1 = BitConverter.GetBytes(affixes[i * 2]);
            byte[] affix2 = BitConverter.GetBytes(affixes[i * 2 + 1]);

            packed[i * 3] = affix1[0];
            packed[i * 3 + 1] = affix2[0];
            packed[i * 3 + 2] = (byte)((affix1[1] << 4) | (affix2[1] & 0xF));
        }
        writer.Write(packed, 0, packed.Length);
    }
}

public class PacketError : Exception
{
    public string PacketName { get; }
    public string FieldName { get; }

    public PacketError(string packetName, string fieldName, Exception innerException)
        : base($"Error in packet '{packetName}', field '{fieldName}'", innerException)
    {
        PacketName = packetName;
        FieldName = fieldName;
    }
}