using System;
using System.IO;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Database;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x03, 0x0C)]
    public class PingResponse : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            Logger.Write("[HI!] 收到 {0} Ping回应 ", context.User.Username);
        }

        #endregion
    }
}