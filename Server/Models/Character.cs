using System;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;

using PSO2SERVER.Packets;
using PSO2SERVER.Database;

namespace PSO2SERVER.Models
{
    public class Character
    {
        public enum ClassType : byte
        {
            Unknown = 0xFF,
            Hunter = 0,
            Ranger,
            Force,
            Fighter,
            Gunner,
            Techer,
            Braver,
            Bouncer,
            Challenger,
            Summoner,
            BattleWarrior,
            Hero,
            Phantom,
            Etole,
            Luster,
        }

    //    每个枚举成员的值是通过位移操作计算的：

    //Hunter：1 << 0，即 1（0x0001）
    //Ranger：1 << 1，即 2（0x0002）
    //Force：1 << 2，即 4（0x0004）
    //Fighter：1 << 3，即 8（0x0008）
    //Gunner：1 << 4，即 16（0x0010）
    //Techer：1 << 5，即 32（0x0020）
    //Braver：1 << 6，即 64（0x0040）
    //Bouncer：1 << 7，即 128（0x0080）
    //Challenger：1 << 8，即 256（0x0100）
    //Summoner：1 << 9，即 512（0x0200）
    //BattleWarrior：1 << 10，即 1024（0x0400）
    //Hero：1 << 11，即 2048（0x0800）
    //Phantom：1 << 12，即 4096（0x1000）
    //Etole：1 << 13，即 8192（0x2000）
    //Luster：1 << 14，即 16384（0x4000）

        [Flags]
        public enum ClassTypeField : ushort
        {
            Unknown = 0xFF,
            None = 0,
            Hunter = 1 << 0,
            Ranger = 1 << 1,
            Force = 1 << 2,
            Fighter = 1 << 3,
            Gunner = 1 << 4,
            Techer = 1 << 5,
            Braver = 1 << 6,
            Bouncer = 1 << 7,
            Challenger = 1 << 8,
            Summoner = 1 << 9,
            BattleWarrior = 1 << 10,
            Hero = 1 << 11,
            Phantom = 1 << 12,
            Etole = 1 << 13,
            Luster = 1 << 14
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CharParam
        {
            public int character_id;
            public int player_id;
            public uint unk1;
            public uint voice_type;
            public ushort unk2;
            public short voice_pitch;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JobEntry
        {
            public ushort level;
            public ushort level2; // Usually the same as the above, what is this used for?
            public uint exp;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Entries
        {
            public JobEntry 
                hunter
                , ranger
                , force
                , fighter
                , gunner
                , techer
                , braver
                , bouncer
                , Challenger
                , Summoner
                , BattleWarrior
                , Hero
                , Phantom
                , Etole
                , Luster
                , unk16
                , unk17
                , unk18
                , unk19
                , unk20
                , unk21
                , unk22
                , unk23
                , unk24
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct JobParam
        {
            public ClassType mainClass;
            public ClassType subClass;
            public ushort unk2;
            public ClassTypeField enabledClasses;
            public ushort unk3;
            public Entries entries; //TODO: Make this a fixed array
            public fixed ushort unk_maxlevel[15];
        }

        public enum RunAnimation : ushort
        {
            Walking = 9,
            Hovering = 11
        }

        public enum Race : ushort
        {
            Unknown = 0xFFFF,
            Human = 0,
            Newman,
            Cast,
            Dewman,
        }

        public enum Gender : ushort
        {
            Unknown = 0xFFFF,
            Male = 0,
            Female,
        }

        public struct AccessoryData
        {
            public sbyte Value1;
            public sbyte Value2;
            public sbyte Value3;
        }

        public enum SkinColor
        {
            RaceDefined,
            Human,
            Deuman,
            Cast
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HSVColor
        {
            public ushort hue, saturation, value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Figure
        {
            public ushort x, y, z; // Great naming, SEGA
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct LooksParam
        {
            public RunAnimation running_animation;
            public Race race;
            public Gender gender;
            public ushort Muscule;
            public Figure Body;
            public Figure Arms;
            public Figure Legs;
            public Figure Chest;
            public Figure FaceShape;
            public Figure FaceParts;
            public Figure Eyes;
            public Figure NoseSize;
            public Figure NoseHeight;
            public Figure Mouth;
            public Figure Ears;
            public Figure Neck;
            public Figure Waist;
            public Figure Body2;
            public Figure Arms2;
            public Figure Legs2;
            public Figure Chest2;
            public Figure Neck2;
            public Figure Waist2;
            public fixed byte Unk1[0x20];
            public fixed byte Unk2[0x0A];
            public AccessoryData Acc1Location;
            public AccessoryData Acc2Location;
            public AccessoryData Acc3Location;
            public AccessoryData Acc4Location;
            public HSVColor UnkColor;
            public HSVColor CostumeColor;
            public HSVColor MainColor;
            public HSVColor Sub1Color;
            public HSVColor Sub2Color;
            public HSVColor Sub3Color;
            public HSVColor EyeColor;
            public HSVColor HairColor;
            public fixed byte Unk3[0x20];
            public fixed byte Unk4[0x10];
            public ushort CostumeId;
            public ushort BodyPaint1;
            public ushort StickerId;
            public ushort RightEyeId;
            public ushort EyebrowId;
            public ushort EyelashId;
            public ushort FaceId1;
            public ushort FaceId2;
            public ushort Facemakeup1Id;
            public ushort HairstyleId;
            public ushort Acc1Id;
            public ushort Acc2Id;
            public ushort Acc3Id;
            public ushort Facemakeup2Id;
            public ushort LegId;
            public ushort ArmId;
            public ushort Acc4Id;
            public fixed byte Unk5[0x04];
            public ushort BodyPaint2;
            public ushort LeftEyeId;
            public fixed byte Unk6[0x12];
            public AccessoryData Acc1Size;
            public AccessoryData Acc2Size;
            public AccessoryData Acc3Size;
            public AccessoryData Acc4Size;
            public AccessoryData Acc1Rotation;
            public AccessoryData Acc2Rotation;
            public AccessoryData Acc3Rotation;
            public AccessoryData Acc4Rotation;
            public ushort Unk7;
            public fixed byte Unk8[0x08];
            public SkinColor SkinColorType;
            public sbyte EyebrowThickness;
        }

        // Probably more info than this
        [Key]
        public int Id { get; set; }

        public int CharacterId { get; set; }
        public int player_id { get; set; }
        public uint unk1 { get; set; }
        public uint voice_type { get; set; }
        public ushort unk2 { get; set; }
        public short voice_pitch { get; set; }

        public string Name { get; set; }

        public byte[] LooksBinary
        {
            get
            {
                PacketWriter w = new PacketWriter();
                w.WriteStruct(Looks);
                return w.ToArray();
            }

            set
            {
                Looks = Helper.ByteArrayToStructure<LooksParam>(value);
            }

        }

        public LooksParam Looks { get; set; }

        public uint unk3 { get; set; }

        public byte[] JobsBinary
        {
            get
            {
                PacketWriter w = new PacketWriter();
                w.WriteStruct(Jobs);
                return w.ToArray();
            }

            set
            {
                Jobs = Helper.ByteArrayToStructure<JobParam>(value);
            }

        }

        public JobParam Jobs { get; set; }

        public string unk4 { get; set; }

        public virtual Player Player { get; set; }
    }


    public struct PSOLocation
    {
        public float RotX, RotY, RotZ, RotW, PosX, PosY, PosZ; // RotX, RotY, RotZ, and RotW make up a Quaternion

        public PSOLocation(float RotX, float RotY, float RotZ, float RotW, float PosX, float PosY, float PosZ)
        {
            this.RotX = RotX;
            this.RotY = RotY;
            this.RotZ = RotZ;
            this.RotW = RotW;

            this.PosX = PosX;
            this.PosY = PosY;
            this.PosZ = PosZ;
        }

        public override string ToString()
        {
            return String.Format("Rot: ({0}, {1}, {2}, {3}) Loc: ({4}, {5}, {6})"
                , RotX, RotY, RotZ, RotW, PosX, PosY, PosZ);
        }
    }
}
