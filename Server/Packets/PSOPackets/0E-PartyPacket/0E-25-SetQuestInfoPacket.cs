using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PSO2SERVER.Packets.PSOPackets.QuestAvailablePacket;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class SetQuestInfoPacket : Packet
    {
        /// <summary>
        /// Name ID of the quest.
        /// </summary>
        public uint Name { get; set; }

        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public ushort Unk3 { get; set; }
        public ushort Unk4 { get; set; }

        /// <summary>
        /// Player who accepted the quest.
        /// </summary>
        public ObjectHeader Player { get; set; }

        public uint[] Unk5 { get; set; } = new uint[5];
        public byte Unk6 { get; set; }
        public byte Unk7 { get; set; }
        public byte Unk8 { get; set; }

        /// <summary>
        /// Quest difficulty.
        /// </summary>
        public byte Diff { get; set; }

        /// <summary>
        /// Quest type.
        /// </summary>
        public QuestType QuestType { get; set; }

        QuestDefiniton questdef;

        public SetQuestInfoPacket(QuestDefiniton questdef, int accountid)
        {
            this.questdef = questdef;
            Player = new ObjectHeader((uint)accountid, ObjectType.Player);
        }
        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write(questdef.questNameString);
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Write(Unk3);
            writer.Write(Unk4);
            //writer.Write((ushort)1);
            writer.WriteStruct(Player);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(Unk6);
            writer.Write(Unk7);
            writer.Write(Unk8);
            writer.Write(Diff);
            writer.Write((byte)QuestType);
            return writer.ToArray();
        }
        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x0E, 0x25, PacketFlags.None);
        }
    }
}