using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x0E, 0x19)]
    class ChatStatusHandler : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            //var info = string.Format("[<--] 接收到的数据 (hex): ");
            //Logger.WriteHex(info, data);

            var reader = new PacketReader(data);
            var obj = reader.ReadStruct<ObjectHeader>();
            var status = reader.ReadUInt32();

            context.SendPacket(new ChatStatusPacket(obj.ID, status));
        }
    }
}