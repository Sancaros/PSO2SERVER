﻿using PSO2SERVER.Models;
using PSO2SERVER.Object;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x03, 0x3C)]
    class TeleportLobbyToCafe : PacketHandler
    {
        /// (0x03, 0x3C) Move Lobby -> Cafe.
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context._account == null)
                return;

            // Dunno what these are yet.
            context.SendPacket(0x11, 0xA, 0x0, BitConverter.GetBytes(context._account.AccountId));
            context.SendPacket(0x1E, 0xC, 0x0, BitConverter.GetBytes(101));

            Map dstMap = ZoneManager.Instance.MapFromInstance("cafe", "lobby");
            if(dstMap != null)
            {
                dstMap.SpawnClient(context, dstMap.GetDefaultLocation());
            }
            else
            {
                Logger.WriteError("cafe 区域不存在");
                return;
            }

        }
    }
}
