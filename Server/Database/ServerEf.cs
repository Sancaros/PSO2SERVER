﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using MySql.Data.EntityFramework;
using PSO2SERVER.Models;
using static PSO2SERVER.Models.Character;

namespace PSO2SERVER.Database
{
    public class ServerInfo
    {
        [Key, MaxLength(255)]
        public string Info { get; set; }

        public string Setting { get; set; }
    }

    public class Teleport
    {
        [Key, Column(Order = 1)]
        public string ZoneName { get; set; }

        [Key, Column(Order = 2)]
        public int ObjectID { get; set; }

        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }
        public float RotW { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
    }

    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string SettingsIni { get; set; }
        public string Cpu_Info { get; set; }
        public string Video_Info { get; set; }
    }

    public class PlayerSystemInfo
    {
        [Key]
        public int PlayerId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string SettingsIni { get; set; }
    }

    public class NPC
    {
        [Key, Column(Order = 1)]
        public int EntityID { get; set; }
        [Key, Column(Order = 2)]
        public string ZoneName { get; set; }

        public string NPCName { get; set; }

        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }
        public float RotW { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
    }

    public class GameObject
    {
        [Key, Column(Order = 1)]
        public int ObjectID { get; set; }
        [Key, Column(Order = 2)]
        public string ZoneName { get; set; }

        public string ObjectName { get; set; }

        public byte[] ObjectFlags { get; set; }

        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }
        public float RotW { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
    }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ServerEf : DbContext
    {
        public DbSet<ServerInfo> ServerInfos { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Teleport> Teleports { get; set; }
        public DbSet<NPC> NPCs { get; set; }
        public DbSet<GameObject> GameObjects { get; set; }

        public ServerEf()
            : base(
                string.Format("server={0};port={1};database={2};username={3};password={4}",
                                         ServerApp.Config.DatabaseAddress,
                                         ServerApp.Config.DatabasePort,
                                         ServerApp.Config.DatabaseName,
                                         ServerApp.Config.DatabaseUsername,
                                         ServerApp.Config.DatabasePassword)
                  )
        {
        }

        public void SetupDB()
        {
            try
            {
                foreach (
                    var f in
                        Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "/Resources/sql/scripts/", "*.sql"))
                {
                    Logger.WriteInternal("[DBC] 执行数据库脚本 {0}", f);
                    Database.ExecuteSqlCommand(File.ReadAllText(f));
                }
                var revision = ServerInfos.Find("Revision");
                if (revision == null)
                {
                    revision = new ServerInfo { Info = "Revision", Setting = "0" };
                    ServerInfos.Add(revision);

                    //TODO Possibly move this somewhere else?
                    Database.ExecuteSqlCommand("ALTER TABLE Account AUTO_INCREMENT=10000000");
                }
                SaveChanges();

                Logger.WriteInternal("[DBC] 加载数据集修订的数据库 {0}", revision.Setting);
            }
            catch (Exception ex)
            {
                Logger.WriteException("数据库异常", ex);
            }
        }

        public bool TestDatabaseConnection2()
        {
            try
            {
                using (var context = new ServerEf())
                {
                    // 执行一个简单的查询来测试数据库连接
                    context.Database.ExecuteSqlCommand("SELECT 1");

                    Logger.WriteInternal("[DBT] 数据库连接成功。");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException("数据库连接异常", ex);
                return false;
            }
        }

        public bool TestDatabaseConnection()
        {
            try
            {
                using (var context = new ServerEf())
                {
                    context.Database.Initialize(force: false);

                    Logger.WriteInternal("[DBT] 数据库连接成功。");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException("数据库连接异常", ex);
                return false;
            }
        }
    }
}