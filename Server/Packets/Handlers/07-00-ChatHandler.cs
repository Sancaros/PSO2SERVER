using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSO2SERVER.Packets.Handlers
{

    [PacketHandlerAttr(0x07, 0x00)]
    public class ChatHandler : PacketHandler
    {
        //public uint Unk1 { get; set; }
        //public uint Unk2 { get; set; }
        //public uint Unk3 { get; set; }
        //public byte[] VerId { get; set; } = new byte[0x20];
        //public List<NetInterface> Interfaces { get; set; } = new List<NetInterface>();
        //public byte[] Unk4 { get; set; } = new byte[0x90];
        //public byte[] Unk5 { get; set; } = new byte[0x10];
        //public Language TextLang { get; set; }
        //public Language VoiceLang { get; set; }
        //public Language TextLang2 { get; set; }
        //public Language LangLang { get; set; }
        //public string Language { get; set; } = new string(' ', 0x10);
        //public uint Unk6 { get; set; }
        //public uint Unk7 { get; set; }
        //public uint Magic1 { get; set; }
        //public byte[] Unk8 { get; set; } = new byte[0x20];
        //public byte[] Unk9 { get; set; } = new byte[0x44];
        //public string Username { get; set; } = new string(' ', 0x40);
        //public string Password { get; set; } = new string(' ', 0x40);
        //public uint Unk10 { get; set; }
        //public string Unk11 { get; set; }
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context.Character == null)
                return;
            var info = string.Format("[接收] 接收到的数据 (hex): ");
            Logger.WriteHex(info, data);

            var reader = new PacketReader(data, position, size);
            reader.BaseStream.Seek(0xC, SeekOrigin.Begin);
            var channel = reader.ReadUInt32();
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
                        Logger.WriteCommand(null, "[CMD] {0} 发送指令 {1}", context.User.Username, full);
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

                var writer = new PacketWriter();
                writer.WritePlayerHeader((uint) context.User.PlayerId);
                writer.Write(channel);
                writer.WriteUtf16(message, 0x9D3F, 0x44);

                data = writer.ToArray();

                foreach (var c in Server.Instance.Clients)
                {
                    if (c.Character == null || c.CurrentZone != context.CurrentZone)
                        continue;

                    c.SendPacket(0x07, 0x00, 0x44, data);
                }
            }
        }

        #endregion
    }
    public enum MessageChannel : byte
    {
        // Map channel.
        Map = 0,

        // Party channel.
        Party = 1,

        // Alliance channel.
        Alliance = 2,

        // Whisper channel.
        Whisper = 3,

        // Group channel.
        Group = 4,

        // Undefined channel.
        Undefined = 0xFF
    }

}