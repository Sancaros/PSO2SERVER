using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Security.Cryptography;

using PSO2SERVER.Database;
using PSO2SERVER.Packets.Handlers;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PSO2SERVER
{
    internal class ServerApp
    {
        public static ServerApp Instance { get; private set; }
        
        // Will be using these around the app later [KeyPhact]
        public const string ServerName = "Phantasy Star Online 2 Server";
        public const string ServerShortName = "PSO2";
        public const string ServerAuthor = "Sancaros (https://github.com/Sancaros/PSO2SERVER)";
        public const string ServerCopyright = "(C) 2024 Sancaros.";
        public const string ServerLicense = "All licenced under AGPL.";
        public const string ServerVersion = "v0.1.2";
        public const string ServerVersionName = "Sancaros";

        public const int ServerShipProtNums = 10;
        public const int ServerShipProt = 12000;

        public const int ServerShipListProtNums = 10;
        public const int ServerShipListProt = 12099;

        public const string ServerSettingsKey = "Resources\\settings.txt";
        public const string ServerMemoryPacket = "Resources\\setMemoryPacket.bin";

        // 密钥BLOB格式
        public const string ServerPrivateKeyBlob = "key\\privateKey.blob";
        public const string ServerPublicKeyBlob = "key\\publicKey.blob";
        public const string ServerSEGAKeyBlob = "key\\SEGAKey.blob";

        // 密钥PEM格式 来自Schthack
        public const string ServerPrivatePem = "key\\privateKey.pem";
        public const string ServerSEGAPem = "key\\SEGAKey.pem";

        public static IPAddress BindAddress = IPAddress.Parse("127.0.0.1");
        public static Config Config;
        public static ConsoleSystem ConsoleSystem;

        public List<QueryServer> QueryServers = new List<QueryServer>();
        public Server Server;

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            string libDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib");
            string assemblyPath = Path.Combine(libDir, new AssemblyName(args.Name).Name + ".dll");

            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            return null;
        }

        public static void Main(string[] args)
        {
            // 设置 AssemblyResolve 事件处理程序
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssembly);

            Config = new Config();

            ConsoleSystem = new ConsoleSystem { Thread = new Thread(ConsoleSystem.StartThread) };
            ConsoleSystem.Thread.Start();

            // Setup function exit handlers to guarentee Exit() is run before closing
            Console.CancelKeyPress += Exit;
            AppDomain.CurrentDomain.ProcessExit += Exit;

            try
            {
                for (var i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-b":
                        case "--bind-address":
                            if (++i < args.Length)
                            {
                                var value = args[i];
                                try
                                {
                                    if (IPAddress.TryParse(value, out IPAddress ipAddress))
                                    {
                                        // IP address is valid
                                        BindAddress = ipAddress;
                                    }
                                    else
                                    {
                                        // Not an IP address, try resolving as a domain name
                                        var addresses = Dns.GetHostAddresses(value);
                                        if (addresses.Length > 0)
                                        {

                                            // Prefer IPv4 addresses over IPv6
                                            ipAddress = addresses.FirstOrDefault(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ?? addresses[0];
                                            BindAddress = ipAddress; // Use the first resolved IP address
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
                            break;

                        case "-s":
                        case "--size":
                            var splitArgs = args[++i].Split(',');
                            var width = int.Parse(splitArgs[0]);
                            var height = int.Parse(splitArgs[1]);
                            if (width < ConsoleSystem.Width)
                            {
                                Logger.WriteWarning("[ARG] Capping console width to {0} columns", ConsoleSystem.Width);
                                width = ConsoleSystem.Width;
                            }
                            if (height < ConsoleSystem.Height)
                            {
                                Logger.WriteWarning("[ARG] Capping console height to {0} rows", ConsoleSystem.Height);
                                height = ConsoleSystem.Height;
                            }
                            ConsoleSystem.SetSize(width, height);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException("An error has occurred while parsing command line parameters", ex);
            }

            // Check for settings.txt [AIDA]
            if (!File.Exists(ServerSettingsKey))
            {
                // If it doesn't exist, throw an error and quit [AIDA]
                Logger.WriteError("[ERR] 载入 {0} 文件错误. 按任意键退出.", ServerSettingsKey);
                Console.ReadKey();
                Environment.Exit(0);
            }

            Instance = new ServerApp();

            Instance.GenerateKeys();

            // Fix up startup message [KeyPhact]
            Logger.WriteHeader();
            Logger.Write(ServerName + " - " + ServerVersion + " (" + ServerVersionName + ")");
            Logger.Write("作者 " + ServerAuthor);
            //Logger.Write(ServerLicense);

            Thread.Sleep(1000);
            //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<ServerEf>());
            _ = Instance.StartAsync();
        }

        public void GenerateKeys()
        {
            // Process private key files
            KeyLoader.ProcessKeyFiles(ServerPrivatePem, ServerPrivateKeyBlob, true, File.Exists(ServerPrivatePem));

            // Process SEGA public key files
            KeyLoader.ProcessKeyFiles(ServerSEGAPem, ServerSEGAKeyBlob, false, File.Exists(ServerSEGAPem));

            // Process general RSA keys
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Process private and public RSA keys
                KeyLoader.GenerateAndSaveKeyIfNotExists(rsa, ServerPrivateKeyBlob, true);
                KeyLoader.GenerateAndSaveKeyIfNotExists(rsa, ServerPublicKeyBlob, false);
            }
        }

        public async Task StartAsync()
        {
            var startTime = DateTime.Now;  // 记录启动开始时间

            Server = new Server();

            await InitializeConfigurationAsync();

            await InitializeDatabaseAsync();

            await InitializeQueryServers(QueryMode.AuthList, "认证", ServerShipProt, ServerShipProtNums);

            await InitializeQueryServers(QueryMode.ShipList, "舰船", ServerShipListProt, ServerShipListProtNums);

            Logger.WriteInternal("服务器启动完成 " + DateTime.Now);

            var endTime = DateTime.Now;  // 记录启动结束时间
            var duration = endTime - startTime;  // 计算启动耗时

            Logger.WriteInternal($"服务器启动耗时: {duration.TotalSeconds} 秒");  // 记录启动耗时

            Server.Run();
        }

        private async Task InitializeConfigurationAsync()
        {
            await Task.Run(() =>
            {
                Config.Load();
                PacketHandlers.LoadPacketHandlers();
            });
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                Logger.WriteInternal("[DBC] 载入数据库...");
                using (var db = new ServerEf())
                {
                    await Task.Run(() =>
                    {
                        db.TestDatabaseConnection();
                        db.SetupDB();
                    });
                    Logger.WriteInternal("[DBC] 数据库初始化完成。");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException("[DBC] 数据库初始化异常", ex);
                throw; // 重新抛出异常，或者根据需要处理它
            }
        }

        public async Task InitializeQueryServers(QueryMode queryMode, string portname, int port, int portnums)
        {
            await Task.Run(() =>
            {
                if (portnums <= 0)
                    portnums = 1;

                if (portnums > 0)
                {
                    for (var i = 0; i < portnums; i++)
                    {
                        QueryServers.Add(new QueryServer(queryMode, portname, port + (100 * i)));
                    }
                }
            });
        }

        private static void Exit(object sender, EventArgs e)
        {
            // Save the configuration
            Config.Save();
        }
    }
}