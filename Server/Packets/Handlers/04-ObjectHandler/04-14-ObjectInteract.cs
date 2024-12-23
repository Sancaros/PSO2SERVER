﻿using PSO2SERVER.Database;
using PSO2SERVER.Models;
using PSO2SERVER.Object;
using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x04, 0x14)]
    class ObjectInteract : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            PacketReader reader = new PacketReader(data);
            reader.ReadBytes(12); // Padding MAYBE???????????
            ObjectHeader srcObject = reader.ReadStruct<ObjectHeader>();
            byte[] someBytes = reader.ReadBytes(4); // Dunno what this is yet.
            ObjectHeader dstObject = reader.ReadStruct<ObjectHeader>(); // Could be wrong
            reader.ReadBytes(16); // Not sure what this is yet
            string command = reader.ReadAscii(0xD711, 0xCA);
            PSOObject srcObj;
            if(srcObject.ObjectType == ObjectType.Object)
            {
                srcObj = ObjectManager.Instance.getObjectByID(context.CurrentZone.Name, srcObject.ID);
            }
            else if(srcObject.ObjectType == ObjectType.Player)
            {
                srcObj = new PSOObject
                {
                    Header = srcObject,
                    Name = "Account"
                };
            }
            else
            {
                srcObj = null;
            }

            Logger.WriteInternal("[OBJ] {0} (ID {1}) <{2}> --> Ent {3} (ID {4})", srcObj.Name, srcObj.Header.ID, command, (ObjectType)dstObject.ObjectType, dstObject.ID);

            // TODO: Delete this code and do this COMPLETELY correctly!!!
            if (command == "Transfer" && context.CurrentZone.Name == "lobby")
            {
                // Try and get the teleport definition for the object...
                using (var db = new ServerEf())
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
                    var teleporterEndpoint = db.Teleports.Find("lobby", (int)srcObject.ID);

                    if (teleporterEndpoint == null)
                    {
                        Logger.WriteError("[OBJ] Teleporter for {0} in {1} does not contain a destination!", srcObj.Header.ID, "lobby");
                        // Teleport Account to default point
                        context.SendPacket(new TeleportTransferPacket(srcObj, new PSOLocation(0f, 1f, 0f, -0.000031f, -0.417969f, 0.000031f, 134.375f)));
                        // Unhide player
                        context.SendPacket(new ObjectActionPacket(dstObject, srcObject, new ObjectHeader(), new ObjectHeader(), "Forwarded"));
                    }
                    else
                    {
                        PSOLocation endpointLocation = new PSOLocation()
                        {
                            RotX = teleporterEndpoint.RotX,
                            RotY = teleporterEndpoint.RotY,
                            RotZ = teleporterEndpoint.RotZ,
                            RotW = teleporterEndpoint.RotW,
                            PosX = teleporterEndpoint.PosX,
                            PosY = teleporterEndpoint.PosY,
                            PosZ = teleporterEndpoint.PosZ,
                        };
                        // Teleport Account
                        context.SendPacket(new TeleportTransferPacket(srcObj, endpointLocation));
                        // Unhide player
                        context.SendPacket(new ObjectActionPacket(dstObject, srcObject, new ObjectHeader(), new ObjectHeader(), "Forwarded")); 
                    }
                }
            }

            if (command == "READY")
            {
                context.SendPacket(new ObjectActionPacket(new ObjectHeader((uint)context._account.AccountId, ObjectType.Player), srcObj.Header, srcObj.Header,
                    new ObjectHeader(), "FavsNeutral"));
                context.SendPacket(new ObjectActionPacket(new ObjectHeader((uint)context._account.AccountId, ObjectType.Player), srcObj.Header, srcObj.Header,
                    new ObjectHeader(), "AP")); // Short for Appear, Thanks Zapero!
            }

            if (command == "Sit")
            {
                foreach (var client in Server.Instance.Clients)
                {
                    if (client.Character == null || client == context)
                        continue;

                    client.SendPacket(new ObjectActionPacket(new ObjectHeader((uint)client._account.AccountId, ObjectType.Player), srcObj.Header,
                        new ObjectHeader(dstObject.ID, ObjectType.Player), new ObjectHeader(), "SitSuccess"));
                }
            }
        }
    }

}
