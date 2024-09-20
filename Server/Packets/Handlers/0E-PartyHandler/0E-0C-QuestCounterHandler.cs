using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x0E, 0x0C)]
    class QuestDifficultyStartHandler : PacketHandler
    {
        // Go go maximum code duplication (for now)
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            QuestDefiniton def = new QuestDefiniton
            {
                dateOrSomething = "2012/01/05",
                needsToBeNonzero = 0x00000020,
                getsSetToWord = 0x0000000B,
                questNameString = 30010,
                playTime = (byte)QuestListPacket.EstimatedTime.Short,
                partyType = (byte)QuestListPacket.PartyType.SinglePartyQuest,
                difficulties = (byte)QuestListPacket.Difficulties.Normal | (byte)QuestListPacket.Difficulties.hard | (byte)QuestListPacket.Difficulties.VeryHard | (byte)QuestListPacket.Difficulties.SuperHard,
                requiredLevel = 1,
                // Not sure why but these need to be set for the quest to be enabled
                field_FF = 0xF1,
                field_101 = 1
            };

            QuestDifficultyPacket.QuestDifficulty diff = new QuestDifficultyPacket.QuestDifficulty
            {
                dateOrSomething = "2012/01/05",
                something = 0x20,
                something2 = 0x0B,
                questNameString = 30010,

                // These are likely bitfields
                something3 = 0x00030301
            };

            var quest = new Quest("arks_010120")
            {
                questDef = def
            };
            context.currentParty.currentQuest = quest;
            context.SendPacket(new SetQuestInfoPacket(def, context.User));
            context.SendPacket(new QuestStartPacket(def, diff));
            
        }
    }
}
