using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x07, 0x3D)]
    class _07_3D_UNK : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var info = string.Format("[<--] 接收到的数据 (hex): {0} 字节", data.Length);
            Logger.WriteHex(info, data);
        }
    }
}
