using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PSO2SERVER.Database;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x0)]
    public class Login : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data, position, size);

            //var info = string.Format("[<--] 接收到的数据 (hex): ");
            //Logger.WriteHex(info, data);

            reader.BaseStream.Seek(0x2C, SeekOrigin.Current);

            var macCount = reader.ReadMagic(0x5E6, 107);
            reader.BaseStream.Seek(0x1C * macCount, SeekOrigin.Current);

			reader.BaseStream.Seek(0x280, SeekOrigin.Current);

            var username = reader.ReadFixedLengthAscii(0x60);
            var password = reader.ReadFixedLengthAscii(0x60);

            //var username = "sancaros";
            //var password = "12345678";

            //Logger.Write("用户名 {0} 密码 {1}", username, password);

            // What am I doing here even
            using (var db = new ServerEf())
            {
                var users = from u in db.Players
                            where u.Username.ToLower().Equals(username.ToLower())
                            select u;


                var error = "";
                Player user;

                if (!users.Any())
                {
                    // Check if there is an empty field
                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        error = "用户名或密码为空.";
                        user = null;
                    }
                    // Check for special characters
                    else if (!Regex.IsMatch(username, "^[a-zA-Z0-9 ]*$", RegexOptions.IgnoreCase))
                    {
                        error = "用户名不能包含特殊字符\n请只使用字母和数字.";
                        user = null;
                    }
                    else // We're all good!
                    {
                        // Insert new player into database
                        user = new Player
                        {
                            Username = username.ToLower(),
                            Password = BCrypt.Net.BCrypt.HashPassword(password),
                            Nickname = username.ToLower(),
                            // Since we can't display the nickname prompt yet, just default it to the username
                            SettingsIni = File.ReadAllText(ServerApp.ServerSettingsKey)
                        };
                        db.Players.Add(user);
                        db.SaveChanges();

                        context.SendPacket(0x11, 0x1e, 0x0, new byte[0x44]); // Request nickname
                    }
                }
                else
                {
                    user = users.First();

                    if(password != user.Password)
                    {
                        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                        {
                            error = "密码错误.";
                            user = null;
                        }
                    }
                }

                /* Mystery packet
                var mystery = new PacketWriter();
                mystery.Write((uint)100);
                SendPacket(0x11, 0x49, 0, mystery.ToArray()); */

                // Login response packet
               
                context.SendPacket(new LoginDataPacket("Server Block 1", error, (user == null) ? (uint)0 : (uint)user.PlayerId));

                if (user == null)
                    return;

                // Settings packet
                var settings = new PacketWriter();
                settings.WriteAscii(user.SettingsIni, 0x54AF, 0x100);
                context.SendPacket(0x2B, 2, 4, settings.ToArray());

                context.User = user;

            }

            if (ServerApp.Config.motd != "")
            {
                context.SendPacket(new SystemMessagePacket(ServerApp.Config.motd, SystemMessagePacket.MessageType.AdminMessageInstant));
            }

        }

        #endregion
    }
}