using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using PSO2SERVER.Database;
using PSO2SERVER.Models;

namespace PSO2SERVER.Object
{
    class ObjectManager
    {
        private static readonly ObjectManager instance = new ObjectManager();

        private Dictionary<String, Dictionary<ulong, PSOObject>> zoneObjects = new Dictionary<string, Dictionary<ulong, PSOObject>>();

        private Dictionary<ulong, PSOObject> allTheObjects = new Dictionary<ulong, PSOObject>();

        private ObjectManager() { }

        public static ObjectManager Instance
        {
            get
            {
                return instance;
            }
        }

        public PSOObject[] GetObjectsForZone(string zone)
        {
            if (zone == "tpmap") // Return empty object array for an tp'd map for now (We spawn in a teleporter manually)
            {
                return new PSOObject[0];
            }
            if (!zoneObjects.ContainsKey(zone))
            {
                Dictionary<ulong, PSOObject> objects = new Dictionary<ulong, PSOObject>();

                // Collect from db
                using (var db = new ServerEf())
                {
                    var dbObjects = from dbo in db.GameObjects
                                    where dbo.ZoneName == zone
                                    select dbo;

                    foreach(var dbObject in dbObjects)
                    {
                        var newObject = PSOObject.FromDBObject(dbObject);
                        objects.Add(newObject.Header.ID, newObject);
                        allTheObjects.Add(newObject.Header.ID, newObject);
                        Logger.WriteInternal("[OBJ] 从数据库中载入对象 {0} 所属区域 {1}.", newObject.Name, zone);
                    }
                }

                // Fallback
                if (objects.Count < 1 && Directory.Exists("Resources\\objects\\" + zone))
                {
                    Logger.WriteWarning("[OBJ] 数据库中没有为该区域定义的对象 {0}, 回退到文件系统!", zone);
                    var objectPaths = Directory.GetFiles("Resources\\objects\\" + zone);
                    Array.Sort(objectPaths);
                    foreach (var path in objectPaths)
                    {
                        if (Path.GetExtension(path) == ".bin")
                        {
                            var newObject = PSOObject.FromPacketBin(File.ReadAllBytes(path));
                            objects.Add(newObject.Header.ID, newObject);
                            allTheObjects.Add(newObject.Header.ID, newObject);
                            Logger.WriteInternal("[OBJ] BIN文件系统载入对象 ID {0} 名称 {1} 坐标: ({2}, {3}, {4})", newObject.Header.ID, newObject.Name, newObject.Position.PosX,
                                newObject.Position.PosY, newObject.Position.PosZ);
                        }
                        else if (Path.GetExtension(path) == ".json")
                        {
                            var newObject = JsonConvert.DeserializeObject<PSOObject>(File.ReadAllText(path));
                            objects.Add(newObject.Header.ID, newObject);
                            allTheObjects.Add(newObject.Header.ID, newObject);
                            Logger.WriteInternal("[OBJ] JSON文件系统载入对象 ID {0} 名称 {1} 坐标: ({2}, {3}, {4})", newObject.Header.ID, newObject.Name, newObject.Position.PosX,
                                newObject.Position.PosY, newObject.Position.PosZ);
                        }
                    } 
                }

                zoneObjects.Add(zone, objects);

            }

            return zoneObjects[zone].Values.ToArray();

        }

        internal PSONPC[] getNPCSForZone(string zone)
        {
            List<PSONPC> npcs = new List<PSONPC>();
            using (var db = new ServerEf())
            {
                var dbNpcs = from n in db.NPCs
                             where n.ZoneName == zone
                             select n;

                foreach (NPC npc in dbNpcs)
                {
                    PSONPC dNpc = new PSONPC();
                    dNpc.Header = new ObjectHeader((uint)npc.EntityID, EntityType.Object);
                    dNpc.Position = new PSOLocation(npc.RotX, npc.RotY, npc.RotZ, npc.RotW, npc.PosX, npc.PosY, npc.PosZ);
                    dNpc.Name = npc.NPCName;

                    npcs.Add(dNpc);
                    if (!zoneObjects[zone].ContainsKey(dNpc.Header.ID))
                    {
                        zoneObjects[zone].Add(dNpc.Header.ID, dNpc);
                    }
                    if (!allTheObjects.ContainsKey(dNpc.Header.ID))
                        allTheObjects.Add(dNpc.Header.ID, dNpc);
                }
            }

            return npcs.ToArray();
        }

        internal PSOObject getObjectByID(string zone, uint ID)
        {
            //FIXME: This has been commented out because we were getting object errors with possible shared objects? That or it was just object 1 which is an edge case.
            //if(!zoneObjects.ContainsKey(zone) || !zoneObjects[zone].ContainsKey(ID))
            //{
            //    throw new Exception(String.Format("Object ID {0} does not exist in {1}!", ID, zone));
            //}

            //return zoneObjects[zone][ID];
            return getObjectByID(ID);
        }

        internal PSOObject getObjectByID(uint ID)
        {
            if (!allTheObjects.ContainsKey(ID))
            {
                Logger.WriteWarning("[OBJ] 客户端请求的对象 {0} 服务端未解析. 等待分析.", ID);
                return new PSOObject() { Header = new ObjectHeader(ID, EntityType.Object), Name = "Unknown" };
            }

            return allTheObjects[ID];
        }
    }
}
