using System;
using System.Runtime.InteropServices;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Models
{
    public enum QuestType : byte
    {
        Unk0 = 0,
        Extreme = 1,
        ARKS = 3,
        LimitedTime = 4,
        ExtremeDebug = 5,
        Blank1 = 6,
        NetCafe = 8,
        WarmingDebug = 9,
        Blank2 = 10,
        Advance = 11,
        Expedition = 12,
        FreeDebug = 13,
        ArksDebug = 14,
        Challenge = 16,
        Urgent = 17,
        UrgentDebug = 18,
        TimeAttack = 19,
        TimeDebug = 20,
        ArksDebug2 = 21,
        ArksDebug3 = 22,
        ArksDebug4 = 23,
        ArksDebug5 = 24,
        ArksDebug6 = 25,
        ArksDebug7 = 26,
        ArksDebug8 = 27,
        ArksDebug9 = 28,
        ArksDebug10 = 29,
        Blank3 = 30,
        Recommended = 32,
        Ultimate = 33,
        UltimateDebug = 34,
        AGP = 35,
        Bonus = 36,
        StandardTraining = 37,
        HunterTraining = 38,
        RangerTraining = 39,
        ForceTraining = 40,
        FighterTraining = 41,
        GunnerTraining = 42,
        TechterTraining = 43,
        BraverTraining = 44,
        BouncerTraining = 45,
        SummonerTraining = 46,
        AutoAccept = 47,
        Ridroid = 48,
        CafeAGP = 49,
        BattleBroken = 50,
        BusterDebug = 51,
        Poka12 = 52,
        StoryEP1 = 55,
        Buster = 56,
        HeroTraining = 57,
        Amplified = 58,
        DarkBlastTraining = 61,
        Endless = 62,
        Blank4 = 64,
        PhantomTraining = 65,
        AISTraining = 66,
        DamageCalculation = 68,
        EtoileTraining = 69,
        Divide = 70,
        Stars1 = 71,
        Stars2 = 72,
        Stars3 = 73,
        Stars4 = 74,
        Stars5 = 75,
        Stars6 = 76,
    }

    [Flags]
    public enum QuestTypeAvailable : UInt64
    {
        None = 0x0000000000000000,
        All = 0xFFFFFFFFFFFFFFFF,
        Extreme = 0x0000000000000002,
        StoryEP1 = 0x0000000000000004,
        Arks = 0x0000000000000008,
        Limited = 0x0000000000000010,
        ExtremeDebug = 0x0000000000000020,
        Blank1 = 0x0000000000000040,
        StoryEP2 = 0x0000000000000080,
        NetCafeLimited = 0x0000000000000100,
        ポカポカDebug = 0x0000000000000200,
        Blank2 = 0x0000000000000400,
        Advance = 0x0000000000000800,
        FreeField = 0x0000000000001000,
        FreeDebug = 0x0000000000002000,
        ArksDebug1 = 0x0000000000004000,
        StoryDebug = 0x0000000000008000,
        Challange = 0x0000010000010000,
        Emergency = 0x0000020000020000,
        EmergencyDebug = 0x0000040000040000,
        TimeAttack = 0x0000080000080000,
        TimeDebug = 0x0000000000100000,
        ArksDebug2 = 0x0000000000200000,
        ArksDebug3 = 0x0000000000400000,
        ArksDebug4 = 0x0000000000800000,
        ArksDebug5 = 0x0000000001000000,
        ArksDebug6 = 0x0000000002000000,
        ArksDebug7 = 0x0000000004000000,
        ArksDebug8 = 0x0000000008000000,
        ArksDebug9 = 0x0000000010000000,
        ArksDebug10 = 0x0000000020000000,
        Blank3 = 0x0000000040000000,
        StoryEP3 = 0x0000000080000000,
        Featured = 0x0000000100000000,
        Ultimate = 0x0000000200000000,
        UltimateDebug = 0x0000000400000000,
        NotSet = 0x0000000800000000,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct QuestDefiniton
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32 - 8)]
        public string dateOrSomething;
        public int field_18;
        public int field_1C;
        public int needsToBeNonzero;
        public int alsoGetsSetToDword;
        public UInt16 getsSetToWord;
        public UInt16 moreWordSetting;
        public int questNameString;
        public int field_30;
        public int field_34;
        public int field_38;
        public int field_3C;
        public int field_40;
        public int field_44;
        public int field_48;
        public int field_4C;
        public int field_50;
        public int field_54;
        public int field_58;
        public int field_5C;
        public int field_60;
        public int field_64;
        public int field_68;
        public int field_6C;
        public int field_70;
        public int field_74;
        public int field_78;
        public int field_7C;
        public int field_80;
        public int field_84;
        public int field_88;
        public int field_8C;
        public int field_90;
        public int field_94;
        public int field_98;
        public UInt16 field_9C;
        public byte field_9E;
        public byte field_9F;
        public int field_A0;
        public int field_A4;
        public int field_A8;
        public int field_AC;
        public int field_B0;
        public int field_B4;
        public int field_B8;
        public int field_BC;
        public int field_C0;
        public int field_C4;
        public int field_C8;
        public int field_CC;
        public int field_D0;
        public int field_D4;
        public int field_D8;
        public int field_DC;
        public int field_E0;
        public int field_E4;
        public int field_E8; // Maybe a flags
        public int field_EC;
        public UInt16 field_F0;
        public UInt16 field_F2;
        public UInt16 questBitfield1;
        public byte playTime;
        public byte partyType;
        public byte difficulties;
        public byte difficultiesCompleted;
        public byte field_FA;
        public byte field_FB;
        public byte requiredLevel;
        public byte field_FD;
        public byte field_FE;
        public byte field_FF;
        public byte field_100;
        public byte field_101;
        public byte field_102;
        public byte field_103;
        public byte field_104;
        public byte field_105;
        public UInt16 field_106;
        public int field_108;
        public int field_10C;
        public short field_110;
        public byte field_112;
        public byte field_113;
        public QuestDefThing field_114_1;
        public QuestDefThing field_114_2;
        public QuestDefThing field_114_3;
        public QuestDefThing field_114_4;
        public QuestDefThing field_114_5;
        public QuestDefThing field_114_6;
        public QuestDefThing field_114_7;
        public QuestDefThing field_114_8;
        public QuestDefThing field_114_9;
        public QuestDefThing field_114_10;
        public QuestDefThing field_114_11;
        public QuestDefThing field_114_12;
        public QuestDefThing field_114_13;
        public QuestDefThing field_114_14;
        public QuestDefThing field_114_15;
        public QuestDefThing field_114_16;
    }

    public class Quest
    {
        public int seed;
        public string name;
        public QuestDefiniton questDef;


        public Quest(string name)
        {
            this.name = name;
        }
    }

    public struct QuestDefThing
    {
        public int field_0;
        public int field_4;
        public byte field_8;
        public byte field_9;
        public UInt16 field_A;
    }
}