using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x2B, 0x00)]
    class SettingsRequest : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            //var info = string.Format("[<--] 接收到的数据 (hex): {0} 字节", data.Length);
            //Logger.WriteHex(info, data);

            context.SendPacket(new LoadSettingsPacket(context._account.AccountId));
        }
    }
}
