using System;
using System.Runtime.InteropServices;

using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    class QuestDifficultyPacket : Packet
    {
        private QuestDifficulty[] questdiffs;

        public QuestDifficultyPacket(QuestDifficulty[] questdiffs)
        {
            // Setup dummy difficulty entries
            for (int i = 0; i < questdiffs.Length; i++)
            {
                QuestDifficultyEntry difficulty = new QuestDifficultyEntry
                {
                    ReqLevel = 1,
                    SubClassReqLevel = 0,
                    MonsterLevel = 1,
                    Unk1 = 1,
                    AbilityAdj = 0,
                    DmgLimit = 0,
                    TimeLimit = 0,
                    TimeLimit2 = 0,
                    SuppTarget = 0xFFFFFFFF,
                    Unk2 = 7,
                    Enemy1 = 0xFFFFFFFF,
                    Unk3 = 3,
                    Enemy2 = 0xFFFFFFFF,
                    Unk4 = 3
                };

                questdiffs[i].difficulty1 = difficulty;
                questdiffs[i].difficulty2 = difficulty;
                questdiffs[i].difficulty3 = difficulty;
                questdiffs[i].difficulty4 = difficulty;
                questdiffs[i].difficulty5 = difficulty;
                questdiffs[i].difficulty6 = difficulty;
                questdiffs[i].difficulty7 = difficulty;
                questdiffs[i].difficulty8 = difficulty;
            }

            this.questdiffs = questdiffs;
        }

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            
            writer.WriteMagic((uint)questdiffs.Length, 0x292C, 0x5B);
            foreach (QuestDifficulty d in questdiffs)
            {
                writer.WriteStruct(d);
            }
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x0B, 0x1A, PacketFlags.PACKED);
        }

        //Size: 308 bytes, confirmed in unpacker
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public unsafe struct QuestDifficulty
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string dateOrSomething;
            public ObjectHeader quest_obj;
            public uint name_id;
            public byte area;
            public byte planet;
            public byte unk1;
            public byte unk2;
            public QuestDifficultyEntry difficulty1;
            public QuestDifficultyEntry difficulty2;
            public QuestDifficultyEntry difficulty3;
            public QuestDifficultyEntry difficulty4;
            public QuestDifficultyEntry difficulty5;
            public QuestDifficultyEntry difficulty6;
            public QuestDifficultyEntry difficulty7;
            public QuestDifficultyEntry difficulty8;
        }

        //Size: 32, confirmed in ctor TODO
        public struct QuestDifficultyEntry
        {
            public byte ReqLevel;
            public byte SubClassReqLevel;
            public byte MonsterLevel;
            public byte Unk1;
            public uint AbilityAdj;
            public uint DmgLimit;
            public uint TimeLimit;
            public uint TimeLimit2;
            public uint SuppTarget;
            public uint Unk2;
            public uint Enemy1;
            public uint Unk3;
            public uint Enemy2;
            public uint Unk4;
        }
    }
}
