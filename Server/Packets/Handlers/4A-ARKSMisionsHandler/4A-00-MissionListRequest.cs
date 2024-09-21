using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x4A, 0x00)]
    class MissionListRequest : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if(context.Character == null)
                return;

            //var info = string.Format("[<--] 接收到的数据 (hex): {0} 字节", data.Length);
            //Logger.WriteHex(info, data);

            //Mission mission = new Mission();

            //context.SendPacket(new ARKSMissionListPacket(mission));
        }
    }
}
