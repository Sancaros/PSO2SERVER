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
                                              Path.DirectorySeparatorChar + "Server.cfg";

        // Settings
        [ConfigComment("The address to bind to")]
        public IPAddress BindAddress = IPAddress.Loopback;

        [ConfigComment("The prefix to check for to send a command from the client to the server")]
        public string CommandPrefix = "|";

        [ConfigComment("Address of the database server")]
        public string DatabaseAddress = "127.0.0.1";

        [ConfigComment("Port of the database server")]
        public string DatabasePort = "3306";

        [ConfigComment("Name of the database which contains the Server data")]
        public string DatabaseName = "pso2server";

        [ConfigComment("Username for logging into the database server")]
        public string DatabaseUsername = "root";

        [ConfigComment("Password for logging into the database server")]
        public string DatabasePassword = "root";

        [ConfigComment("Message of the day to display to users upon login.")]
        public string motd = "Wellcom PSO2SERVER";

        [ConfigComment("Time in seconds to perform a ping of all connected clients to the server")]
        public double PingTime = 60;

        [ConfigComment("Enable foreground colors for console text (Unstable on linux)")]
        public bool UseConsoleColors = true;

        [ConfigComment("Log the data sent and recieved from packets")]
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
                        Logger.WriteWarning("[CFG] 发现分割大小不正确的配置行");
                        continue;
                    }

                    var field = fields.FirstOrDefault(o => o.Name == split[0]);
                    if (field != null)
                        ParseField(field, split[1]);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException("[CFG] 设置文件载入错误", ex);
            }

            // Display all settings
            DisplaySettings();

            // Some settings require manual refreshing
            SettingsChanged();

            Logger.WriteInternal("[CFG] 设置文件载入完成");
        }

        private void DisplaySettings()
        {
            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                Logger.WriteInternal($"[CFG] 设置项 {field.Name} = {value}");
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
                Logger.WriteInternal("[CFG] 设置已保存");
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