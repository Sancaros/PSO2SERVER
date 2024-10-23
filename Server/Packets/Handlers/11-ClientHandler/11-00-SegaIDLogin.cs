using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PSO2SERVER.Database;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Models;
using System.Text;
using System.Collections.Generic;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x00)]
    public class SegaIDLogin : PacketHandler
    {
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }
        public byte[] VerId { get; set; } = new byte[0x20];
        public List<NetInterface> Interfaces { get; set; } = new List<NetInterface>();
        public byte[] Unk4 { get; set; } = new byte[0x90];
        public byte[] Unk5 { get; set; } = new byte[0x10];
        public Language TextLang { get; set; }
        public Language VoiceLang { get; set; }
        public Language TextLang2 { get; set; }
        public Language LangLang { get; set; }
        public string LanguageCode { get; set; } = new string('\0', 0x10);
        public uint Unk6 { get; set; }
        public uint Unk7 { get; set; }
        public uint Magic1 { get; set; }
        public byte[] Unk8 { get; set; } = new byte[0x20];
        public byte[] Unk9 { get; set; } = new byte[0x44];
        public string Username { get; set; } = new string('\0', 0x40);
        public string Password { get; set; } = new string('\0', 0x40);
        public uint Unk10 { get; set; }
        public string Unk11 { get; set; } = string.Empty;

        public List<NetInterface> ReadNetInterfaces(PacketReader reader, uint count)
        {
            var interfaces = new List<NetInterface>();
            for (uint i = 0; i < count; i++)
            {
                var netInterface = new NetInterface();
                netInterface.ReadFromStream(reader);
                interfaces.Add(netInterface);
            }
            return interfaces;
        }

        public void ReadFromStream(PacketReader reader)
        {
            Unk1 = reader.ReadUInt32();
            Unk2 = reader.ReadUInt32();
            Unk3 = reader.ReadUInt32();
            VerId = reader.ReadBytes(0x20);

            var macCount = reader.ReadMagic(0x5E6, 107);
            // Assuming Interfaces is populated somehow
            // e.g. Interfaces = ReadNetInterfaces(reader);
            Interfaces = ReadNetInterfaces(reader, macCount);

            // Read the fixed length fields
            reader.BaseStream.Seek(0x14, SeekOrigin.Current);
            Unk4 = reader.ReadBytes(0x90);
            reader.BaseStream.Seek(0x10, SeekOrigin.Current);
            Unk5 = reader.ReadBytes(0x10);
            reader.BaseStream.Seek(0x10, SeekOrigin.Current);
            TextLang = (Language)reader.ReadByte(); // Adjust based on actual type
            VoiceLang = (Language)reader.ReadByte(); // Adjust based on actual type
            TextLang2 = (Language)reader.ReadByte(); // Adjust based on actual type
            LangLang = (Language)reader.ReadByte(); // Adjust based on actual type
            reader.BaseStream.Seek(0x8, SeekOrigin.Current);
            LanguageCode = Encoding.ASCII.GetString(reader.ReadBytes(0x10)).TrimEnd('\0');
            Unk6 = reader.ReadUInt32();
            Unk7 = reader.ReadUInt32();
            Magic1 = reader.ReadUInt32();
            Unk8 = reader.ReadBytes(0x20);
            Unk9 = reader.ReadBytes(0x44);

            // Read Username and Password
            reader.BaseStream.Seek(0x120, SeekOrigin.Current);
            Username = Encoding.ASCII.GetString(reader.ReadBytes(0x40)).TrimEnd('\0');
            reader.BaseStream.Seek(0x20, SeekOrigin.Current);
            Password = Encoding.ASCII.GetString(reader.ReadBytes(0x40)).TrimEnd('\0');
            Unk10 = reader.ReadUInt32();
            Unk11 = Encoding.ASCII.GetString(reader.ReadBytes(0x40)).TrimEnd('\0'); // Adjust size if needed
        }

        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data, position, size);

            ReadFromStream(reader);

            //var info = string.Format("[<--] 接收到的数据 (hex): {0}字节", data.Length);
            //Logger.WriteHex(info, data);

            //Logger.Write("用户名 {0} 密码 {1} - {2}", Username, Password, BCrypt.Net.BCrypt.HashPassword(Password));

            // What am I doing here even
            using (var db = new ServerEf())
            {
                var users = from u in db.Account
                            where u.Username.ToLower().Equals(Username.ToLower())
                            select u;


                var error = "";
                Account user;

                if (!users.Any())
                {
                    // Check if there is an empty field
                    if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                    {
                        error = "用户名或密码为空.";
                        user = null;
                    }
                    // Check for special characters
                    else if (!Regex.IsMatch(Username, "^[a-zA-Z0-9 ]*$", RegexOptions.IgnoreCase))
                    {
                        error = "用户名不能包含特殊字符\n请只使用字母和数字.";
                        user = null;
                    }
                    else // We're all good!
                    {
                        // 直接插入新账户至数据库
                        user = new Account
                        {
                            Username = Username.ToLower(),
                            Password = BCrypt.Net.BCrypt.HashPassword(Password),
                            Nickname = Username.ToLower(),
                            // Since we can't display the nickname prompt yet, just default it to the username
                            SettingsIni = File.ReadAllText(ServerApp.ServerSettingsKey)
                        };
                        db.Account.Add(user);
                        db.SaveChanges();

                        context.SendPacket(0x11, 0x1e, 0x0, new byte[0x44]); // Request nickname
                    }
                }
                else
                {
                    user = users.First();

                    if (Password != user.Password)
                    {
                        if (Password == "")
                        {
                            error = "密码为空.";
                            user = null;
                        }
                        else
                        if (!BCrypt.Net.BCrypt.Verify(Password, user.Password))
                        {
                            error = "密码错误.";
                            user = null;
                        }
                    }
                }

                context.SendPacket(new LoginDataPacket("Server AuthList 1", error, (user == null) ? (uint)0 : (uint)user.AccountId));

                //Mystery packet
                //var mystery = new PacketWriter();
                //mystery.Write((uint)100);
                //context.SendPacket(0x11, 0x49, 0, mystery.ToArray());

                // SegaIDLogin response packet

                if (user == null)
                {
                    return;
                }

                context._account = user;

            }

            if (ServerApp.Config.motd != "")
            {
                context.SendPacket(new SystemMessagePacket(ServerApp.Config.motd, SystemMessagePacket.MessageType.AdminMessageInstant));
            }

        }

        #endregion
    }
    public class NetInterface
    {
        /// <summary>
        /// Interface status.
        /// </summary>
        public uint State { get; set; }

        /// <summary>
        /// Interface MAC address.
        /// </summary>
        public string Mac { get; set; } = new string('\0', 0x18); // 以字符串形式存储

        public void ReadFromStream(PacketReader reader)
        {
            State = reader.ReadUInt32();
            Mac = Encoding.ASCII.GetString(reader.ReadBytes(0x18)).TrimEnd('\0');
        }
    }
    public enum Language
    {
        Japanese = 0,
        English = 1
    }
}