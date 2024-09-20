using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x0E, 0x29)]
    class PlayerIsNotBusyState : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context.Character == null)
                return;
            //var info = string.Format("[<--] 接收到的数据 (hex): ");
            //Logger.WriteHex(info, data);

            foreach (var c in Server.Instance.Clients)
            {
                if (c.Character == null || c.CurrentZone != context.CurrentZone)
                    continue;

                c.SendPacket(new NewBusyStatePacket(context._account.AccountId, BusyState.NotBusy));
            }
        }
    }
}
