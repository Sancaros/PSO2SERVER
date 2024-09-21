﻿using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class SetQuestInfoPacket : Packet
    {

        QuestDefiniton questdef;
        Database.Account p;

        public SetQuestInfoPacket(QuestDefiniton questdef, Database.Account p)
        {
            this.questdef = questdef;
            this.p = p;
        }
        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write(questdef.questNameString);
            writer.Write(0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.WriteStruct(new ObjectHeader((uint)p.AccountId, ObjectType.Player));
            writer.Write(0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write(0);
            writer.Write(0);
            return writer.ToArray();
        }
        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x0E, 0x25, PacketFlags.None);
        }
    }
}