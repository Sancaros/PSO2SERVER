using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Party;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x0B, 0xCD)]
    class AcceptStoryQuestHandler : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            //var info = string.Format("[<--] 接收到的数据 (hex): ");
            //Logger.WriteHex(info, data);

            var reader = new PacketReader(data);
            var name_id = reader.ReadUInt32();
            var unk = reader.ReadUInt32();

            Logger.Write("任务编号: " + name_id + " unk: " + unk);

            //PartyManager.instance.CreateNewParty(context);

            // 告诉客户端切换到加载界面
            context.SendPacket(new LoadingScreenTransitionPacket());
        }
    }
}
