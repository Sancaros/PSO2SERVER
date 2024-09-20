using PSO2SERVER.Database;
using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class LoadSettingsPacket : Packet
    {
        private int _PlayerId;
        public LoadSettingsPacket(int PlayerId)
        {
            _PlayerId = PlayerId;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var writer = new PacketWriter();

            using (var db = new ServerEf())
            {
                try
                {
                    // 使用 FirstOrDefault 以避免异常
                    var player = db.Account.FirstOrDefault(w => w.AccountId == _PlayerId);

                    if (player == null)
                    {
                        Logger.WriteError("未找到 AccountId: {0}", _PlayerId);
                        // 可以选择返回一个特定的错误包或空数组
                        return Array.Empty<byte>();
                    }

                    // 确保 SettingsIni 不为 null
                    if (player.SettingsIni != null)
                    {
                        writer.WriteAscii(player.SettingsIni, 0x54AF, 0x100);
                    }
                    else
                    {
                        Logger.WriteError("AccountId: {0} 的 SettingsIni 为 null, 载入默认值", _PlayerId);
                        player.SettingsIni = File.ReadAllText(ServerApp.ServerSettingsKey);

                        // 保存更改并捕获可能的异常
                        db.SaveChanges();

                        // 可以选择如何处理此情况
                        writer.WriteAscii(player.SettingsIni, 0x54AF, 0x100);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteError("构建数据包时发生异常: {0}", ex.Message);
                    // 处理异常情况，例如返回错误包
                }
            }

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x2B, 0x02, PacketFlags.PACKED);
        }

        #endregion
    }
}