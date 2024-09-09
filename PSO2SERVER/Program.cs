﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Security.Cryptography;

using PSO2SERVER.Database;
using PSO2SERVER.Packets.Handlers;

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
        public const string ServerVersion = "v0.1.1";
        public const string ServerVersionName = "Sancaros";

        public static IPAddress BindAddress = IPAddress.Parse("127.0.0.1");
        public static Config Config;
        public static ConsoleSystem ConsoleSystem;

        public List<QueryServer> QueryServers = new List<QueryServer>();
        public Server Server;

        public static void Main(string[] args)
        {
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
                                BindAddress = IPAddress.Parse(args[i]);
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
            if (!File.Exists("Resources/settings.txt"))
            {
                // If it doesn't exist, throw an error and quit [AIDA]
                Logger.WriteError("[ERR] 载入 settings.txt 文件错误. 按任意键退出.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            // Check for Private Key BLOB [AIDA]
            if (!File.Exists("privateKey.blob"))
            {
                // If it doesn't exist, generate a fresh keypair [CK]
                Logger.WriteWarning("[WRN] 未找到 privatekey.blob 文件, 正在生成新的密钥...");
                RSACryptoServiceProvider rcsp = new RSACryptoServiceProvider();
                byte[] cspBlob = rcsp.ExportCspBlob(true);
                byte[] cspBlobPub = rcsp.ExportCspBlob(false);
                FileStream outFile = File.Create("privateKey.blob");
                FileStream outFilePub = File.Create("publicKey.blob");
                outFile.Write(cspBlob, 0, cspBlob.Length);
                outFile.Close();
                outFilePub.Write(cspBlobPub, 0, cspBlobPub.Length);
                outFilePub.Close();
            }

            // Fix up startup message [KeyPhact]
            Logger.WriteHeader();
            Logger.Write(ServerName + " - " + ServerVersion + " (" + ServerVersionName + ")");
            Logger.Write("作者 " + ServerAuthor);
            //Logger.Write(ServerLicense);

            Thread.Sleep(1000);
            //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<ServerEf>());
            Instance = new ServerApp();
            Instance.Start();
        }

        public void Start()
        {
            Logger.WriteInternal("服务器启动完成 " + DateTime.Now);

            Server = new Server();

            Config.Load();

            PacketHandlers.LoadPacketHandlers();

            Logger.WriteInternal("[DB ] 载入数据库...");
            using (var db = new ServerEf())
            {
                db.TestDatabaseConnection();

                db.SetupDB();
            }

            for (var i = 0; i < 10; i++)
                QueryServers.Add(new QueryServer(QueryMode.ShipList, 12099 + (100 * i)));

            Server.Run();
        }

        private static void Exit(object sender, EventArgs e)
        {
            // Save the configuration
            Config.Save();
        }
    }
}