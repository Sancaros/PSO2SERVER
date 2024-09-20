using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x0B, 0x19)]
    class QuestDifficultyRequestHandler : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            QuestDifficultyPacket.QuestDifficulty[] diffs = new QuestDifficultyPacket.QuestDifficulty[1];
            for (int i = 0; i < diffs.Length; i++)
            {
                diffs[i].dateOrSomething = "2012/01/05";
                diffs[i].something = 0x20;
                diffs[i].something2 = 0x0B;
                diffs[i].questNameString = 30010;

                // These are likely bitfields
                diffs[i].something3 = 0x00030301;
            }

            context.SendPacket(new QuestDifficultyPacket(diffs));

            // [K873] I believe this is the correct packet, but it causes an infinite send/recieve loop, we're probably just missing something else
            context.SendPacket(new NoPayloadPacket(0xB, 0x1C));
        }
    }

}
