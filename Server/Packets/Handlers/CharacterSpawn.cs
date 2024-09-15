using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Object;
using PSO2SERVER.Database;
using PSO2SERVER.Zone;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x3E)]
    public class CharacterSpawn : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context.User == null || context.Character == null)
                return;

            // Looks/Jobs
            if (size > 0)
            {
                var reader = new PacketReader(data);

                reader.BaseStream.Seek(0x38, SeekOrigin.Begin);
                context.Character.Looks = reader.ReadStruct<Character.LooksParam>();
                context.Character.Jobs = reader.ReadStruct<Character.JobParam>();

                using(var db = new ServerEf())
                    db.ChangeTracker.DetectChanges();
            }

            Map lobbyMap = ZoneManager.Instance.MapFromInstance("lobby", "lobby");
            lobbyMap.SpawnClient(context, lobbyMap.GetDefaultLocation(), "lobby");
            
            // Unlock Controls
            context.SendPacket(new NoPayloadPacket(0x03, 0x2B));

            //context.SendPacket(File.ReadAllBytes("testbed/237.23-7.210.189.208.30.bin"));

            // Give a blank palette
            context.SendPacket(new PalettePacket());

            // memset packet - Enables menus
            // Also holds event items and likely other stuff too
            var memSetPacket = File.ReadAllBytes(ServerApp.ServerMemoryPacket);
            context.SendPacket(memSetPacket);
        }

        #endregion
    }
}
