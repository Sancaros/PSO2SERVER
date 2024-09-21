using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x07, 0x00)]
    public class ChatHandler : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context.Character == null)
                return;
            var info = string.Format("[<--] 接收到的数据 (hex): ");
            Logger.WriteHex(info, data);

            var reader = new PacketReader(data, position, size);
            var obj = reader.ReadStruct<ObjectHeader>();
            var channel = reader.ReadByte();
            var unk3 = reader.ReadByte();
            var unk4 = reader.ReadInt16();
            reader.BaseStream.Seek(0x4, SeekOrigin.Current);
            var message = reader.ReadUtf16(0x9D3F, 0x44);

            if (message.StartsWith(ServerApp.Config.CommandPrefix))
            {
                var valid = false;

                // Iterate commands
                foreach (var command in ServerApp.ConsoleSystem.Commands)
                {
                    var full = message.Substring(1); // Strip the command chars
                    var args = full.Split(' ');

                    if (command.Names.Any(name => args[0].ToLower() == name.ToLower()))
                    {
                        command.Run(args, args.Length, full, context);
                        valid = true;
                        Logger.WriteCommand(null, "[CMD] {0} 发送指令 {1}", context._account.Username, full);
                    }

                    if (valid)
                        break;
                }

                if (!valid)
                    Logger.WriteClient(context, "[CMD] {0} - 指令不存在", message.Split(' ')[0].Trim('\r'));
            }
            else
            {
                Logger.Write("[CHT] <{0}> 频道{1}说 {2}", context.Character.Name, channel, message);

                foreach (var c in Server.Instance.Clients)
                {
                    if (c.Character == null || c.CurrentZone != context.CurrentZone)
                        continue;

                    c.SendPacket(new ChatPacket((uint)context._account.AccountId, channel, message));
                }
            }
        }

        #endregion
    }
}