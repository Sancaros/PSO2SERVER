using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using PSO2SERVER.Database;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x2B, 0x01)]
    class SavePlayerSettings : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data, position, size);

            var setting = reader.ReadAscii(0xCEF1, 0xB5);

            using (var db = new ServerEf())
            {
                try
                {
                    var player = db.Account.FirstOrDefault(w => w.AccountId == context._account.AccountId);
                    if (player == null)
                    {
                        Logger.WriteError("未找到 AccountId: {0}", context._account.AccountId);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(setting))
                    {
                        Logger.WriteError("新的设置内容无效。");
                        return;
                    }

                    player.SettingsIni = setting;

                    // 保存更改并捕获可能的异常
                    db.SaveChanges();
                }
                catch (DbUpdateException dbEx)
                {
                    Logger.WriteError("数据库更新时发生异常: {0}", dbEx.Message);
                    // 处理数据库更新错误
                }
                catch (Exception ex)
                {
                    Logger.WriteError("构建数据包时发生异常: {0}", ex.Message);
                    // 处理其他异常情况
                }
            }

            //context.SendPacket(new LoadSettingsPacket(context._account.AccountId));
        }
    }
}
