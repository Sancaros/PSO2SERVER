using PSO2SERVER.Models;
using PSO2SERVER.Object;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x03, 0x39)]
    class TeleportLobbyToBridge : PacketHandler
    {
        /// (0x03, 0x39) Move Lobby -> Bridge.
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context.User == null)
                return;

            // Dunno what these are yet.
            context.SendPacket(0x11, 0xA, 0x0, BitConverter.GetBytes(context.User.PlayerId));
            context.SendPacket(0x1E, 0xC, 0x0, BitConverter.GetBytes(101));

            Map bridgeMap = ZoneManager.Instance.MapFromInstance("bridge", "lobby");
            bridgeMap.SpawnClient(context, bridgeMap.GetDefaultLocation());

        }
    }
}
