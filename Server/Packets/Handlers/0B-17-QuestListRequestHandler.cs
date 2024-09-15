using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0xB, 0x17)]
    class QuestListRequestHandler : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            // What am I doing
            QuestDefiniton[] defs = new QuestDefiniton[1];
            for (int i = 0; i < defs.Length; i++)
            {
                defs[i].dateOrSomething = "2012/01/05";
                defs[i].needsToBeNonzero = 0x00000020;
                defs[i].getsSetToWord = 0x0000000B;
                defs[i].questNameString = 30010;
                defs[i].playTime = (byte)QuestListPacket.EstimatedTime.Short;
                defs[i].partyType = (byte)QuestListPacket.PartyType.SinglePartyQuest;
                defs[i].difficulties = (byte)QuestListPacket.Difficulties.Normal | (byte)QuestListPacket.Difficulties.hard | (byte)QuestListPacket.Difficulties.VeryHard | (byte)QuestListPacket.Difficulties.SuperHard;
                defs[i].requiredLevel = 1;
                // Not sure why but these need to be set for the quest to be enabled
                defs[i].field_FF = 0xF1;
                defs[i].field_101 = 1;
            }

            context.SendPacket(new QuestListPacket(defs));
            context.SendPacket(new NoPayloadPacket(0xB, 0x1B));
        }
    }

}
