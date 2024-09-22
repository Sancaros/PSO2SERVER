using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x21, 0x02)]
    class FullPaletteInfoRequest : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var info = string.Format("[<--] 接收到的数据 (hex): {0} 字节", data.Length);
            Logger.WriteHex(info, data);

            var Palette = PSOPalette.Palette.Create();

            context.SendPacket(new FullPaletteInfoPacket(Palette));
        }
    }
}