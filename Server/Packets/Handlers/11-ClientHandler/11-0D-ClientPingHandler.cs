using System;
using System.IO;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x0D)]
    public class ClientPingHandler : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data, position, size);
            var clientTime = reader.ReadUInt64();

            context.SendPacket(new ClientPongPacket(clientTime));
        }

        #endregion
    }
}
