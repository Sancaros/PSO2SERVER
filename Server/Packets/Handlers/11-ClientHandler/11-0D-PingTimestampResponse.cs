using System;
using System.IO;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x0D)]
    public class PingTimestampResponse : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data, position, size);
            var clientTime = reader.ReadUInt64();

            var writer = new PacketWriter();
            writer.Write(clientTime);
            writer.Write(Helper.Timestamp(DateTime.UtcNow));
            context.SendPacket(0x11, 0xE, 0, writer.ToArray());
        }

        #endregion
    }
}
