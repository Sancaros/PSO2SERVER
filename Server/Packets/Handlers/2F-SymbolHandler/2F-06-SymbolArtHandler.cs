using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.Handlers
{

    [PacketHandlerAttr(0x2F, 0x6)]
    class SymbolArtListHandler : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            context.SendPacket(new SymbolArtListPacket(new Models.ObjectHeader((uint)context._account.AccountId, Models.ObjectType.Player)));
        }
    }
}
