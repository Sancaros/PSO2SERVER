using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using PSO2SERVER.Database;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x2D)]
    class SystemInformation : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data, position, size);

            var cpu_info = reader.ReadAscii(0x883D, 0x9F);
            var video_info = reader.ReadAscii(0x883D, 0x9F);
            //reader.BaseStream.Seek(8, SeekOrigin.Current);
            //var vram = reader.ReadAscii(0x883D, 0x9F);
            //var info = string.Format("[<--] 接收到的数据 (hex): {0} 字节", data.Length);
            //Logger.WriteHex(info, data);
            //var windows_version = reader.ReadAscii(0x6C, 190);


            //Logger.Write("Setting 内容: " + windows_version);



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

                    if (!string.IsNullOrWhiteSpace(cpu_info))
                    {
                        player.Cpu_Info = cpu_info;
                    }

                    if (!string.IsNullOrWhiteSpace(video_info))
                    {
                        player.Video_Info = video_info;
                    }

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
