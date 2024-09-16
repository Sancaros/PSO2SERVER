using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x0B, 0x30)]
    class QuestCounterHandler : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            // Not sure what this does yet
            byte[] allTheQuests = new byte[408];

            for (int i = 0; i < allTheQuests.Length; i++)
                allTheQuests[i] = 0xFF;

            context.SendPacket(0xB, 0x22, 0x0, allTheQuests);
        }
    }

}
