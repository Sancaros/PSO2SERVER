using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace PSO2SERVER
{
    public class ConfigComment : Attribute
    {
        public string Comment;

        public ConfigComment(string comment)
        {
            Comment = comment;
        }
    }

    public class Config
    {
        private readonly string _configFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                              Path.DirectorySeparatorChar + "config\\Server.cfg";

        // Settings
        [ConfigComment("服务器对外绑定的地址(支持域名或者IP,优先获取IPV4)")]
        public IPAddress BindAddress = IPAddress.Loopback;

        [ConfigComment("从客户端向服务器发送命令时要检查的前缀")]
        public string CommandPrefix = "|";

        [ConfigComment("数据库地址")]
        public string DatabaseAddress = "127.0.0.1";

        [ConfigComment("数据库端口")]
        public string DatabasePort = "3306";

        [ConfigComment("数据库表名称")]
        public string DatabaseName = "pso2server";

        [ConfigComment("数据库用户名")]
        public string DatabaseUsername = "root";

        [ConfigComment("数据库密码")]
        public string DatabasePassword = "root";

        [ConfigComment("登录时显示给用户的当天消息")]
        public string motd = "Wellcom PSO2SERVER";

        [ConfigComment("对所有连接到服务器的客户机执行ping操作的时间(以秒为单位)")]
        public double PingTime = 60;

        [ConfigComment("为控制台文本启用前景色(在linux上不稳定)")]
        public bool UseConsoleColors = true;

        [ConfigComment("记录从数据包发送和接收的数据")]
        public bool VerbosePackets = false;

        public void Load()
        {
            try
            {
                // No config exists, save a default one
                if (!File.Exists(_configFile))
                {
                    Save(true);
                    return;
                }

                var fields = GetType().GetFields();
                var lines = File.ReadAllLines(_configFile);

                foreach (var option in lines)
                {
                    // Blank Line
                    if (option.Length == 0)
                        continue;

                    // Comment
                    if (option.StartsWith("//"))
                        continue;

                    var split = option.Split('=');

                    // Trim trailing/leading space
                    for (var i = 0; i < split.Length; i++)
                        split[i] = split[i].Trim();

                    // Check length
                    if (split.Length != 2)
                    {
                        Logger.WriteWarning("[设置] 发现分割大小不正确的配置行");
                        continue;
                    }

                    var field = fields.FirstOrDefault(o => o.Name == split[0]);
                    if (field != null)
                        ParseField(field, split[1]);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException("[设置] 设置文件载入错误", ex);
            }

            // Display all settings
            DisplaySettings();

            // Some settings require manual refreshing
            SettingsChanged();

            Logger.WriteInternal("[设置] 设置文件载入完成");
        }

        private void DisplaySettings()
        {
            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                Logger.WriteInternal($"[设置] 设置项 {field.Name} = {value}");
            }
        }

        public void Save(bool silent = false)
        {
            try
            {
                var data = new List<string>();
                var fields = GetType().GetFields();

                foreach (var field in fields)
                    SaveField(field, data);

                File.WriteAllLines(_configFile, data);
            }
            catch (Exception ex)
            {
                Logger.WriteException("保存设置错误", ex);
            }

            if (!silent)
                Logger.WriteInternal("[设置] 设置已保存");
        }

        public void SettingsChanged()
        {
            ServerApp.BindAddress = BindAddress;
            Logger.VerbosePackets = VerbosePackets;
            ServerApp.Instance.Server.PingTimer.Interval = 1000 * PingTime;
        }

        public bool SetField(string name, string value)
        {
            var fields = GetType().GetFields();
            var field = fields.FirstOrDefault(o => o.Name == name);

            if (field != null)
            {
                ParseField(field, value);
                return true;
            }
            return false;
        }

        private void ParseField(FieldInfo field, string value)
        {
            // Bool
            if (field.GetValue(this) is bool)
                field.SetValue(this, bool.Parse(value));

            // Int32
            if (field.GetValue(this) is int)
                field.SetValue(this, int.Parse(value));

            // Float
            if (field.GetValue(this) is float)
                field.SetValue(this, float.Parse(value));

            // Double
            if (field.GetValue(this) is double)
                field.SetValue(this, double.Parse(value));

            // String
            if (field.GetValue(this) is string)
            {
                value = value.Replace("\\n", "\n");
                field.SetValue(this, value);
            }

            // IP Address
            if (field.GetValue(this) is IPAddress)
            {
                try
                {
                    IPAddress ipAddress;
                    if (IPAddress.TryParse(value, out ipAddress))
                    {
                        field.SetValue(this, ipAddress);
                    }
                    else
                    {
                        // Try resolving domain name to IP address
                        var addresses = Dns.GetHostAddresses(value);
                        if (addresses.Length > 0)
                        {
                            // Prefer IPv4 addresses over IPv6
                            ipAddress = addresses.FirstOrDefault(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ?? addresses[0];
                            field.SetValue(this, ipAddress);
                        }
                        else
                        {
                            Logger.WriteError($"No IP addresses found for domain: {value}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteError($"Error resolving domain {value}: {ex.Message}");
                }
            }

            // Add more handling for special/custom types as needed
        }

        private void SaveField(FieldInfo field, List<string> data)
        {
            // Comment
            var attributes = (Attribute[])field.GetCustomAttributes(typeof(ConfigComment), false);
            if (attributes.Length > 0)
            {
                var commentAttr = (ConfigComment)attributes[0];
                data.Add("// " + commentAttr.Comment);
            }

            // IP Address
            if (field.GetValue(this).GetType() == typeof(IPAddress))
            {
                var address = (IPAddress)field.GetValue(this);
                data.Add(field.Name + " = " + address);
            }
            else if (field.GetValue(this).GetType() == typeof(string))
            {
                var str = (string)field.GetValue(this);
                data.Add(field.Name + " = " + str.Replace("\n", "\\n"));
            }
            else // Basic field
            {
                data.Add(field.Name + " = " + field.GetValue(this));
            }
            
            // Leave a blank line between options
            data.Add(string.Empty);

            // Add more handling for special/custom types as needed
        }
    }
}