using System;
using System.Collections.Generic;
using System.IO;

using PSO2SERVER.Models;
using PSO2SERVER.Object;
using PSO2SERVER.Packets;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Zone
{
    public class Map
    {
        public string Name { get; set; }
        public MapType Type { get; set; }
        public PSOObject[] Objects { get; set; }
        public PSONPC[] NPCs { get; set; }
        public ObjectHeader MapHeader { get; set; }

        public int MapID { get; set; }
        public int VariantID { get; set; }

        public MapFlags Flags { get; set; }

        public List<Client> Clients { get; set; }

        public GenParam GenerationArgs { get; set; }

        public string InstanceName { get; set; }

        public enum MapType : int
        {
            Lobby = 4,
            Casino = 11,
            MyRoom = 8,
            TeamRoom = 7,
            ChallengeLobby = 5,
            Campship = 1,
            Quest = 0,
            Other = ~0
        };

        [Flags]
        public enum MapFlags : uint
        {
            None = 0,
            MultiPartyArea = 1,
            Unknown1 = 2,
            EnableMap = 4
        }

        public Map(string name, int id, int variant, MapType type, MapFlags flags)
        {
            Name = name;
            MapID = id;
            VariantID = variant;
            Type = type;
            Flags = flags;
            Clients = new List<Client>();
            GenerationArgs = new GenParam();

            Objects = ObjectManager.Instance.GetObjectsForZone(Name);
            NPCs = ObjectManager.Instance.GetNpcSForZone(Name);
        }

        public PSOLocation GetDefaultLocation()
        {
            PSOLocation location;

            switch (Type)
            {
                case MapType.Lobby:
                    location = new PSOLocation(0f, 1f, 0f, 0f, -0.417969f, 0f, 137.375f);
                    break;

                case MapType.Casino:
                    location = new PSOLocation(0f, 1f, 0f, 0f, 2f, 6f, 102f);
                    break;

                default:
                    location = new PSOLocation(0f, 1f, 0f, 0f, 0f, 0f, 0f);
                    break;
            }

            return location;
        }

        /// <summary>
        /// Spawns a client into a map at a given location
        /// </summary>
        /// <param name="c">Client to spawn into map</param>
        /// <param name="location">Location to spawn client at</param>
        /// <param name="questOveride">If this also sets the quest data, specify the quest name here to load the spawn from the bin rather then generate it.</param>
        public void SpawnClient(Client c, PSOLocation location, string questOveride = "")
        {
            if (Clients.Contains(c))
            {
                return;
            }

            // Set area
            if (questOveride != "") // TODO: This is a temporary hack, fix me!!
            {
                //var setAreaPacket = File.ReadAllBytes("Resources\\quests\\" + questOveride + ".bin");
                //c.SendPacket(0x03, 0x24, 0x04, setAreaPacket);

                c.SendPacket(new LoadingLevelPacket(questOveride));
            }
            else
            {
                //PacketWriter writer = new PacketWriter();
                //writer.WriteStruct(new ObjectHeader(3, EntityType.Map));
                //writer.WriteStruct(new ObjectHeader((uint)c.User.PlayerId, EntityType.Player));
                //writer.Write(0x1); // 8 Zeros
                //writer.Write(0); // 8 Zeros
                //writer.Write(~(uint)Type); // F4 FF FF FF
                //writer.Write(MapID); // Map ID maybe
                //writer.Write((uint)Flags);
                //writer.Write(GenerationArgs.seed); // 81 8F E6 19 (Maybe seed)
                //writer.Write(VariantID); // Randomgen enable / disable maybe
                //writer.Write(GenerationArgs.xsize); // X Size
                //writer.Write(GenerationArgs.ysize); // Y Size
                //writer.Write(1);
                //writer.Write(1);
                //writer.Write(~0); // FF FF FF FF FF FF FF FF
                //writer.Write(0x301);

                //c.SendPacket(0x3, 0x0, 0x0, writer.ToArray());

                var _map = new Map("", MapID, VariantID, Type, Flags);
                _map.GenerationArgs.seed = GenerationArgs.seed;
                _map.GenerationArgs.xsize = GenerationArgs.xsize;
                _map.GenerationArgs.ysize = GenerationArgs.ysize;

                c.SendPacket(new MapTransferPacket(_map, c.User.PlayerId));
            }

            if (c.CurrentZone != null)
            {
                c.CurrentZone.RemoveClient(c);
            }

            //var setPlayerId = new PacketWriter();
            //setPlayerId.WritePlayerHeader((uint)c.User.PlayerId);
            //c.SendPacket(0x06, 0x00, 0, setPlayerId.ToArray());
            c.SendPacket(new SetPlayerIDPacket(c.User.PlayerId));

            // Spawn Character
            c.SendPacket(new CharacterSpawnPacket(c.Character, location));
            c.CurrentLocation = location;
            c.CurrentZone = this;

            // Objects
            foreach (PSOObject obj in Objects)
            {
                c.SendPacket(new ObjectSpawnPacket(obj));
            }

            // NPCs
            foreach (PSONPC npc in NPCs)
            {
                c.SendPacket(new NPCSpawnPacket(npc));
            }

            // Spawn for others, Spawn others for me
            CharacterSpawnPacket spawnMe = new CharacterSpawnPacket(c.Character, location, false);
            foreach (Client other in Clients)
            {
                other.SendPacket(spawnMe);
                c.SendPacket(new CharacterSpawnPacket(other.Character, other.CurrentLocation, false));
            }

            // Unlock Controls
            c.SendPacket(new UnlockControlsPacket()); // Inital spawn only, move this!

            Clients.Add(c);

            Logger.Write("[MAP] {0} 已生成至 {1}.", c.User.Username, Name);

            if (InstanceName != null && ZoneManager.Instance.playerCounter.ContainsKey(InstanceName))
            {
                ZoneManager.Instance.playerCounter[InstanceName] += 1;
            }

        }

        public void RemoveClient(Client c)
        {
            if (!Clients.Contains(c))
                return;

            c.CurrentZone = null;
            Clients.Remove(c);

            foreach (Client other in Clients)
            {
                //PacketWriter writer = new PacketWriter();
                //writer.WriteStruct(new ObjectHeader((uint)other.User.PlayerId, EntityType.Player));
                //writer.WriteStruct(new ObjectHeader((uint)c.User.PlayerId, EntityType.Player));
                //other.SendPacket(0x4, 0x3B, 0x40, writer.ToArray());
                other.SendPacket(new DespawnPlayerPacket(other.User.PlayerId, c.User.PlayerId));
            }

            if (InstanceName != null && ZoneManager.Instance.playerCounter.ContainsKey(InstanceName))
            {
                ZoneManager.Instance.playerCounter[InstanceName] -= 1;
                if (ZoneManager.Instance.playerCounter[InstanceName] <= 0)
                {
                    ZoneManager.Instance.playerCounter.Remove(InstanceName);
                    ZoneManager.Instance.instances.Remove(InstanceName);
                }
            }
        }

        public class GenParam
        {
            public GenParam()
            {
            }

            public GenParam(uint seed, uint x, uint y)
            {
                this.seed = seed;
                this.xsize = x;
                this.ysize = y;
            }
            public uint seed, xsize, ysize;
        }
    }
}